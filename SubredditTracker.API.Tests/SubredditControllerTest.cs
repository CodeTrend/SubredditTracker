using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SubredditTracker.API.Controllers;
using SubredditTracker.API.Interfaces;
using SubredditTracker.Domain.Interfaces;
using System.Net;

namespace SubredditTracker.API.Tests;

public class SubredditControllerTest
{
    [Fact]
    internal void GetSubscription_When_Value_Found_In_Cache_Return_Ok()
    {
        //Arrange
        var cacheMock = new Mock<ICachingService>();
        var logMock = new Mock<ILogger<SubredditController>>();
        var redditServiceMock = new Mock<IRedditDataService>();
        var subredditController = new SubredditController(cacheMock.Object, logMock.Object, redditServiceMock.Object);
        object result;
        cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out result)).Returns(true);

        //Act
        var actionResult = subredditController.GetSubscription();

        //Assert
        var response = actionResult.Result as OkObjectResult;
        Assert.Equal(response?.StatusCode, (int)HttpStatusCode.OK);

    }

    [Fact]
    internal void GetSubscription_When_Value_Not_Found_In_Cache_Return_NotFound()
    {
        //Arrange
        var cacheMock = new Mock<ICachingService>();
        var logMock = new Mock<ILogger<SubredditController>>();
        var redditServiceMock = new Mock<IRedditDataService>();
        var subredditController = new SubredditController(cacheMock.Object, logMock.Object, redditServiceMock.Object);
        object result;
        cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out result)).Returns(false);

        //Act
        var actionResult = subredditController.GetSubscription();

        //Assert
        Assert.True(actionResult != null);
        Assert.Equal(expected: ((NotFoundResult)actionResult.Result!).StatusCode, actual: (int)HttpStatusCode.NotFound);

    }



}
