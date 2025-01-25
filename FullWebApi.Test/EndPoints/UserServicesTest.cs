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
  public async Task SignUp_Should_Return_NewUser()
  {
    var newUser = _fixture.Create<User>();
    _mockUserServices.Setup(service => service.SignUpUser(newUser)).ReturnsAsync(newUser);
    var result = await _mockUserServices.Object.SignUpUser(newUser);

    Assert.NotNull(result);
    Assert.Equal(newUser.Id, result.Id);
  }
}