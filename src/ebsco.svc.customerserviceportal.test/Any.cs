using System;
using System.Collections.Generic;
using System.Text;
using WorkItemTracker;
using ebsco.svc.customerserviceportal.Models;
using System.Security.Claims;

namespace ebsco.svc.customerserviceportal.test
{
    public static class Any
    {
        private static readonly Random _random = new Random();

        public static bool Bool() => _random.Next(0, 1) == 1;

        public static string String(int length = 10)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[_random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        public static int Int(int max = 1000) => _random.Next(max);

        public static byte[] ByteArray(int length = 100)
        {
            var byteArray = new byte[length];

            _random.NextBytes(byteArray);

            return byteArray;
        }

        public static WorkItem WorkItem()
        {
            return new WorkItem
            {
                WorkItemId = Int(),
                CreatedBy = String(),
                CreatedDate = DateTime.UtcNow,
                Title = new WorkItemTitle { TitleName = String() },
                Order = new WorkItemOrder { EbscoOrderNumber = String() },
                WebUserInformation = new WebUser
                {
                    FirstName = String(),
                    LastName = String()
                },
                WorkItemStatusDescription = String(),
                WorkItemTypeDescription = String()
            };
        }

        public static WorkItem[] WorkItems(int length = 10)
        {
            var workItems = new List<WorkItem>();

            for (var i = 0; i < length; i++)
            {
                workItems.Add(WorkItem());
            }

            return workItems.ToArray();
        }

        public static WorkItemDetail WorkItemDetail()
        {
            return new WorkItemDetail
            {
                DetailDescription = String(),
                DetailValue = String(),
                Order = Int(),
                RequestInformationEntity = new RequestInformation
                {
                    TranslationKey = string.Empty
                }
            };
        }

        public static WorkItemDetail[] WorkItemDetails(int length = 5)
        {
            var workItemDetails = new List<WorkItemDetail>();

            for (var i = 0; i < length; i++)
            {
                workItemDetails.Add(WorkItemDetail());
            }

            return workItemDetails.ToArray();
        }

        public static WorkItemDocument WorkItemDocument(int workItemId = 0)
        {
            if (workItemId == 0)
            {
                workItemId = Int();
            }

            return new WorkItemDocument
            {
                CreatedBy = String(),
                CreatedByUserName = String(),
                CreatedDate = DateTime.UtcNow,
                DeleteFlag = Bool(),
                DocumentDescription = String(),
                DocumentId = String(),
                DocumentName = String(),
                DocumentTypeDescription = String(),
                IsHyperlink = Bool(),
                ModifiedBy = String(),
                ModifiedByUserName = String(),
                ModifiedDate = DateTime.UtcNow,
                WorkItemDocumentTypeId = Int(),
                WorkItemId = workItemId
            };
        }

        public static WorkItemDocument[] WorkItemDocuments(int length = 10, int workItemId = 0)
        {
            var workItemDocuments = new List<WorkItemDocument>();
            
            for (var i = 0; i < length; i++)
            {
                workItemDocuments.Add(WorkItemDocument(workItemId));
            }

            return workItemDocuments.ToArray();
        }

        public static Communication Communication(int workItemId = 0)
        {
            if (workItemId == 0)
            {
                workItemId = Int();
            }

            return new Communication
            {
                WorkItemId = workItemId,
                CommunicationId = Int(),
                CreatedByUserName = String(),
                CreatedDate = DateTime.UtcNow,
                Details = String()
            };
        }

        public static Communication[] Communications(int length = 10, int workItemId = 0)
        {
            var communications = new List<Communication>();

            for (var i = 0; i < length; i++)
            {
                communications.Add(Communication(workItemId));
            }

            return communications.ToArray();
        }

        public static WorkItemCommentCreateModel WorkItemCommentCreateModel()
        {
            return new WorkItemCommentCreateModel
            {
                CommentFromContactTypeId = Int(),
                CommentMethodId = Int(),
                CommentToContactTypeId = Int(),
                Details = String(),
                IsInComment = Bool(),
                Organization = String()
            };
        }

        public static WorkItemUserDetails WorkItemUserDetails()
        {
            return new WorkItemUserDetails
            {
                Id = String(),
                Name = String(),
                Email = String(),
                Phone = String(),
                Fax = String()
            };
        }

        public static Claim Claim()
        {
            return new Claim (String(), String());
        }
    }
}
