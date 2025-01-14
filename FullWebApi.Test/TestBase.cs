using AutoFixture;
using AutoFixture.AutoMoq;

namespace FullWebApi.Test;

public abstract class TestBase
{

    protected readonly IFixture Fixture;

    protected TestBase()
    {
        Fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

}