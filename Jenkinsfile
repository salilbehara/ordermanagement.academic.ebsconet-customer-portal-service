def successfulDeployments = []

pipeline {
	agent any

	environment {
		BUILD_NAME = "${env.JOB_NAME}".replaceAll("/","-").toLowerCase()
		BUILD_TAG = "${env.BUILD_NAME}-${env.BUILD_NUMBER}"
		BUILD_PATH = "D:\\Builds\\${env.BUILD_NAME}\\${env.BUILD_NUMBER}\\"
        BIN_DIR = "published\\"
		EISGOSVC_CREDS = credentials('eisgosvc_credentials')
		SITE_BASE_PATH = "D:\\Program Files\\EBSCO\\Service\\"
		LOG_BASE_PATH = "E:\\Logs\\EBSCO\\Service\\"
		SETUP_SCRIPT_PATH = ".\\pipeline\\configure-server.ps1"
		DEPLOYMENT_SCRIPT_PATH = ".\\pipeline\\deploy-to-server.ps1"
		DEPLOYMENT_PACKAGE_PATH="${env.BUILD_PATH}${env.BIN_DIR}"
		ENABLED_PROTOCOLS = "http,https"
		ALLOW_WIN_AUTH = true
		REQUIRE_SSL = true
		START_WEBSITE = false
	}

    stages {
        stage('Build and Run Unit Tests') {
			steps {
                echo "Build tag: ${env.BUILD_TAG}"
				echo "Build path: ${env.BUILD_PATH}"
                
				writeFile file: 'Dockerfile', text: libraryResource('docker\\net-core-build\\2.0\\Dockerfile')
				powershell "docker build --build-arg bin_dir=${env.BIN_DIR} --tag ${env.BUILD_TAG}-i ."
            }
        }

		stage('Copy files to Jenkins server') {
			when {
				branch 'master'
			}

			steps {
				script {
					echo "Creating Docker container: docker create ${env.BUILD_TAG}-c:latest"
					def containerId = powershell(returnStdout: true, script: "docker create ${env.BUILD_TAG}-i:latest").substring(0, 11)
					echo "ContainerId: ${containerId}"

					try {
						powershell "mkdir ${env.BUILD_PATH}"
						powershell "docker cp ${containerId}:ebsco.svc.customerserviceportal\\${env.BIN_DIR} ${env.BUILD_PATH}"
					} finally {
						powershell "docker rm ${containerId}"					
					}	
				}
			}
		}

		stage ('Deploy to Dev1') {
			agent none

			when {
				branch 'master'
			}
			
			environment {				
				SERVER_NAME = "eishqwb05d"	
				FQDN = "dev1.customerservice.api.ebsconet.com"
				SITE_IP = "10.45.149.238"
				ENVIRONMENT = "Development"
				TARGET_LOCATION="${env.SITE_BASE_PATH}\\${env.FQDN}\\"		
			}

			steps {
				prepareForIisDeployment()
				deployIis()
				
				script {
				    successfulDeployments.push('dev1')
				}
			}
		}

		stage ('Deploy to QA') {
			when {
				branch 'master'
			}

			stages {
				stage ('Deploy to EISHQWB05T') {
					agent none
					
					environment {				
						SERVER_NAME = "EISHQWB05T"	
						FQDN = "qa1.customerservice.api.ebsconet.com"
						SITE_IP = "10.45.163.184"
						ENVIRONMENT = "QA"
						TARGET_LOCATION="${env.SITE_BASE_PATH}\\${env.FQDN}\\"				
					}

					steps {
						script {
							input("Deploy to EISHQWB05T?")
						}
						
						prepareForIisDeployment()
						deployIis()
						
						script {
							successfulDeployments.push('QA - EISHQWB05T')
						}
					}
				}	

				stage ('Deploy to EISHQWB07T') {
					agent none
					
					environment {				
						SERVER_NAME = "EISHQWB07T"	
						FQDN = "qa1.customerservice.api.ebsconet.com"
						SITE_IP = "10.45.163.185"
						ENVIRONMENT = "QA"
						TARGET_LOCATION="${env.SITE_BASE_PATH}\\${env.FQDN}\\"				
					}

					steps {
						script {
							input("Deploy to EISHQWB07T?")
						}
						
						prepareForIisDeployment()
						deployIis()
						
						script {
							successfulDeployments.push('QA - EISHQWB07T')
						}
					}
				}							
			}
		}
    }

	post {
		always {
			powershell "docker image remove -f ${env.BUILD_TAG}-i"

		    script {
		        if (successfulDeployments.size() > 0) {
		            successfulDeployments.each {
		                echo "Deployed ${env.BUILD_TAG} to ${it}"
		            }
		        }
		    }
		}
	}
}

void prepareForIisDeployment() {
	powershell '''
		$allowWinAuth = [System.Convert]::ToBoolean($env:ALLOW_WIN_AUTH)
		$requireSsl = [System.Convert]::ToBoolean($env:REQUIRE_SSL)
		$startWebsite = [System.Convert]::ToBoolean($env:START_WEBSITE)

		$securePassword = "$env:EISGOSVC_CREDS_PSW" | ConvertTo-SecureString -AsPlainText -Force -ErrorAction Stop
		$credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $env:EISGOSVC_CREDS_USR,$securePassword -ErrorAction Stop

		echo "Setting up the IIS site on $env:SERVER_NAME"
		Invoke-Command -ComputerName $env:SERVER_NAME -Credential $credentials -Verbose -FilePath $env:SETUP_SCRIPT_PATH -ArgumentList $env:FQDN, $env:SITE_IP, $env:ENABLED_PROTOCOLS, $env:ENVIRONMENT, $env:SITE_BASE_PATH, $env:LOG_BASE_PATH, $allowWinAuth, $requireSsl, $startWebsite -ErrorAction Stop
	'''
}

void deployIis() {
	powershell '''
		$securePassword = "$env:EISGOSVC_CREDS_PSW" | ConvertTo-SecureString -AsPlainText -Force -ErrorAction Stop
		$credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $env:EISGOSVC_CREDS_USR,$securePassword -ErrorAction Stop

		echo "Copying build $env:BUILD_TAG to $env:SERVER_NAME"
		$session = New-PSSession -ComputerName $env:SERVER_NAME -Credential $credentials -ErrorAction Stop
		$sessionId = [int]$session.Id
		Invoke-Expression ". `"$env:DEPLOYMENT_SCRIPT_PATH`" -sessionId $sessionId -sourceLocation `"$env:DEPLOYMENT_PACKAGE_PATH`" -targetLocation `"$env:TARGET_LOCATION`""
		Remove-PsSession $session -ErrorAction Stop

		echo "Starting app pool for $env:FQDN"
		Invoke-Command -ComputerName $env:SERVER_NAME -Credential $credentials -ArgumentList $env:FQDN -ScriptBlock { param($appPoolName) if((Get-WebAppPoolState -Name $appPoolName).Value -ne 'Running')  { Start-WebAppPool -Name $appPoolName } } -ErrorAction Stop
	'''
}