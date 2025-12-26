using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FullWebApi.Api.EndPoints;
using FullWebApi.Application.Interfaces;
using FullWebApi.Domain.Dtos;
using FullWebApi.Domain.Models;
using FullWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FullWebApi.Test.EndPoints;

public class UserServicesTest
{
  private readonly IFixture _fixture;
  private readonly Mock<IUserServices> _mockUserServices;
    
  public UserServicesTest()
  {
    _fixture = new Fixture().Customize(new AutoMoqCustomization());
    _mockUserServices = _fixture.Freeze<Mock<IUserServices>>();         
  }

  [Fact]
  public async Task GetAllUsers_Should_Return_Users()
  {   
    //Prepares the services  
    List<UserDto> newUsers = _fixture.CreateMany<UserDto>(5).ToList();
    _mockUserServices.Setup(service => service.GetAllUsers()).ReturnsAsync(newUsers);

    //Executes the tested method 
    var result = await _mockUserServices.Object.GetAllUsers();

    //Verifies the test results
    Assert.NotNull(result);
    Assert.Equal(5, result.Count());         
  }
  
  [Fact]
  public async Task GetUser_Should_Return_User()
  {
    var user = _fixture.Create<User>();
    _mockUserServices.Setup(service => service.GetUser(user.Id)).ReturnsAsync(user);
    
    var result = await _mockUserServices.Object.GetUser(user.Id);
    
    Assert.NotNull(result);
    Assert.Equal(user.Id, result.Id);
  }

  [Fact] 
  public async Task DeleteUser_Should_Return_True_When_Successful()
  {
    var userId = _fixture.Create<Guid>();
    _mockUserServices.Setup(service => service.DeleteUser(userId)).ReturnsAsync(true);

    var result = await _mockUserServices.Object.DeleteUser(userId);

    Assert.True(result);
  }

  [Fact]
  public async Task UpdateUser_Should_Return_UpdatedUser()
  {
    var user = _fixture.Create<User>();
    _mockUserServices.Setup(service => service.UpdateUser(user)).ReturnsAsync(user);

    var result = await _mockUserServices.Object.UpdateUser(user);

    Assert.NotNull(result);
    Assert.Equal(user.Id, result.Id);
    Assert.Equal(user.Username, result.Username);
    Assert.Equal(user.Email, result.Email);
  }
  
  [Fact]
  public async Task CreateUser_Should_Return_NewUser()
  {
    var newUser = _fixture.Create<User>();
    var expectedResult = (User: newUser, ErrorResponse: (object?)null);
    _mockUserServices.Setup(service => service.SignUpUser(newUser)).ReturnsAsync(expectedResult);
    
    var result = await _mockUserServices.Object.SignUpUser(newUser);

    Assert.NotNull(result.User);
    Assert.Null(result.ErrorResponse);
    Assert.Equal(newUser.Id, result.User.Id);
  }
}