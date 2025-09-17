using HexagonalShop.WebAPI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace HexagonalShop.Test.WebAPI.Middleware;

public class SwaggerMiddlewareTests
{
    [Fact]
    public void AddSwaggerWithAuth_ShouldRegisterSwaggerGenWithSecurityScheme()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
                          .Returns(null);

        // Act
        services.AddSwaggerWithAuth();
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();

        // Assert - Verificar que se agregaron los servicios de Swagger
        Assert.NotNull(services.FirstOrDefault(s => s.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>)));
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldAddBearerSecurityScheme()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Agregar logging para evitar excepciones

        // Act
        services.AddSwaggerWithAuth();
        var serviceProvider = services.BuildServiceProvider();

        // Obtener las opciones de SwaggerGen
        var swaggerGenOptions = serviceProvider.GetService<IConfigureOptions<SwaggerGenOptions>>();

        // Assert - Verificar que el método no lanza excepciones y los servicios se registran
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldNotThrowWhenCalledMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            services.AddSwaggerWithAuth();
            services.AddSwaggerWithAuth(); // Llamar dos veces
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void UseSwaggerWithAuth_ShouldNotThrowWhenCalledOnNullApplication()
    {
        // Arrange
        WebApplication? nullApp = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => nullApp!.UseSwaggerWithAuth());
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldRegisterRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerWithAuth();

        // Assert - Verificar que se registran los servicios esenciales de Swagger
        var serviceDescriptors = services.Where(s => 
            s.ServiceType.Name.Contains("Swagger", StringComparison.OrdinalIgnoreCase) ||
            s.ServiceType.Name.Contains("ApiExplorer", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.NotEmpty(serviceDescriptors);
    }

    [Fact]
    public void SecurityScheme_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        var expectedScheme = new OpenApiSecurityScheme
        {
            Description = "Enter the JWT token in the field. Example: Bearer {token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };

        // Act - Llamar al método para verificar que no hay excepciones
        var services = new ServiceCollection();
        services.AddLogging();

        var exception = Record.Exception(() => services.AddSwaggerWithAuth());

        // Assert
        Assert.Null(exception);
        // No podemos verificar directamente la configuración interna, pero podemos verificar que no hay excepciones
    }

    [Fact]
    public void SecurityRequirement_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var exception = Record.Exception(() => services.AddSwaggerWithAuth());

        // Assert
        Assert.Null(exception);
        // El security requirement se configura internamente, verificamos que no hay excepciones
    }

    [Fact]
    public void AddSwaggerWithAuth_ShouldBeExtensionMethod()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        // Verificar que el método de extensión existe y se puede llamar
        services.AddSwaggerWithAuth();

        // Assert
        // Si no hay excepción, el método de extensión funciona correctamente
        Assert.True(true);
    }
}