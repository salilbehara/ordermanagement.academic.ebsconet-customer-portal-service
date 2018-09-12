using ebsco.svc.customerserviceportal.Controllers;
using Moq;
using WorkItemTracker;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ebsco.svc.customerserviceportal.Models;
using Newtonsoft.Json;
using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Responses;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemsController_GetWorkItemDetails_should_
    {
        [Fact]
        public void return_work_items_details()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                Details = Any.WorkItemDetails(2)
            };

            var expectedWorkItemDetails = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();
            
            var response = GetWorkItemDetails(workItemDetailInfo) as OkObjectResult;

            var actualWorkItemDetails = ((response.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemDetailInfoModel>).Value as WorkItemDetailInfoModel;

            Assert.Equal(200, response.StatusCode);

            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[0].Name, actualWorkItemDetails.WorkItemDetails[0].Name);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[0].Value, actualWorkItemDetails.WorkItemDetails[0].Value);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[0].SortOrder, actualWorkItemDetails.WorkItemDetails[0].SortOrder);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[0].TranslationKey, actualWorkItemDetails.WorkItemDetails[0].TranslationKey);

            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[1].Name, actualWorkItemDetails.WorkItemDetails[1].Name);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[1].Value, actualWorkItemDetails.WorkItemDetails[1].Value);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[1].SortOrder, actualWorkItemDetails.WorkItemDetails[1].SortOrder);
            Assert.Equal(expectedWorkItemDetails.WorkItemDetails[1].TranslationKey, actualWorkItemDetails.WorkItemDetails[1].TranslationKey);
        }

        [Fact]
        public void return_empty_array_when_no_work_item_details_exist_for_work_item_id()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = null,
                NoteText = null,
                NoteLabelTranslationKey = null,
                Details = Any.WorkItemDetails(0)
            };

            var expectedWorkItemDetails = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            var response = GetWorkItemDetails(workItemDetailInfo) as NotFoundObjectResult;

            var actualWorkItemDetails = (response.Value as WorkItemDetailInfo);

            Assert.Equal(404, response.StatusCode);
            Assert.Null(actualWorkItemDetails);
        }        
        
        [Fact]
        public void return_work_item_details_when_note_is_present()
        {
            const int workItemDetailArrayLengthIncludingNoteLabel = 3;

            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = Any.String(),
                NoteText = Any.String(),
                NoteLabelTranslationKey = string.Empty,
                Details = Any.WorkItemDetails(2)
            };

            var expectedWorkItemDetail = new WorkItemDetailModel
            {
                Name = workItemDetailInfo.NoteLabel,
                Value = workItemDetailInfo.NoteText,
                TranslationKey = workItemDetailInfo.NoteLabelTranslationKey
            };

            var response = GetWorkItemDetails(workItemDetailInfo) as OkObjectResult;

            var actualWorkItemDetails = ((response.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemDetailInfoModel>).Value as WorkItemDetailInfoModel;

            Assert.Equal(200, response.StatusCode);
            Assert.Equal(workItemDetailArrayLengthIncludingNoteLabel, actualWorkItemDetails.WorkItemDetails.Length);
            Assert.Contains(JsonConvert.SerializeObject(expectedWorkItemDetail), JsonConvert.SerializeObject(actualWorkItemDetails.WorkItemDetails));
        }

        [Fact]
        public void return_translated_work_item_details_when_translation_key_is_present()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = Any.String(),
                NoteText = Any.String(),
                NoteLabelTranslationKey = Any.String(),
                Details = Any.WorkItemDetails(2)
            };

            var response = GetWorkItemDetails(workItemDetailInfo) as OkObjectResult;

            var actualWorkItemDetails = ((response.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemDetailInfoModel>).Value as WorkItemDetailInfoModel;

            Assert.Equal(200, response.StatusCode);
            Assert.Equal(3, actualWorkItemDetails.WorkItemDetails.Length);
            Assert.Contains("Translated String", actualWorkItemDetails.TranslatedClaimDetailLink);
        }

        private IActionResult GetWorkItemDetails(WorkItemDetailInfo workItemDetailInfo)
        {
            var workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();

            workItemTrackerServiceRepositoryMock.Setup(repository => repository
                           .GetWorkItemDetails(It.IsAny<int>()))
                           .Returns(new GetWorkItemDetailsResponse { WorkItemDetailInfo = workItemDetailInfo });

            var translationsServiceMock = new Mock<ITranslationsService>();

            translationsServiceMock.Setup(repository => repository
                           .TranslateResource(It.IsAny<string>()))
                           .Returns("Translated String");

            var urlHelperMock = new Mock<IUrlHelper>();

            urlHelperMock.Setup(repository => repository
                           .Link(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Any.String());

            return new WorkItemsController(
                workItemTrackerServiceRepositoryMock.Object,
                translationsServiceMock.Object,
                urlHelperMock.Object).GetWorkItemDetails(Any.Int());
        }
    }
}
