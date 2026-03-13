using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Partners;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Tests.Controllers.Builders;

namespace PromoCodeFactory.WebHost.Tests.Controllers
{
    public class PartnersControllerTests
    {
        private readonly Fixture _fixture;

        public PartnersControllerTests()
        {
            _fixture = new Fixture();

            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private PartnersController CreateController(
            Mock<IRepository<Partner>> partnerRepositoryMock = null,
            Mock<IRepository<PartnerLimit>> partnerLimitRepositoryMock = null)
        {
            partnerRepositoryMock ??= new Mock<IRepository<Partner>>();
            partnerLimitRepositoryMock ??= new Mock<IRepository<PartnerLimit>>();

            return new PartnersController(
                partnerRepositoryMock.Object,
                partnerLimitRepositoryMock.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsActive_LimitAdded()
        {
            // Arrange

            var partner = new PartnerBuilder()
                .Active()
                .WithIssuedPromoCodes(5)
                .Build();

            var limitDto = _fixture.Create<SetPartnerLimitDto>();

            var partnerRepositoryMock = new Mock<IRepository<Partner>>();
            partnerRepositoryMock
                .Setup(x => x.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var partnerLimitRepositoryMock = new Mock<IRepository<PartnerLimit>>();

            var controller = CreateController(
                partnerRepositoryMock,
                partnerLimitRepositoryMock);

            // Act

            var result = await controller.SetPartnerPromoCodeLimitAsync(partner.Id, limitDto);

            // Assert

            result.Should().BeOfType<OkObjectResult>();

            partner.NumberIssuedPromoCodes.Should().Be(0);

            partnerLimitRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<PartnerLimit>()),
                Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange

            var partnerRepositoryMock = new Mock<IRepository<Partner>>();
            partnerRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Partner)null);

            var controller = CreateController(partnerRepositoryMock);

            var limitDto = _fixture.Create<SetPartnerLimitDto>();

            // Act

            var result = await controller.SetPartnerPromoCodeLimitAsync(Guid.NewGuid(), limitDto);

            // Assert

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange

            var partner = new PartnerBuilder()
                .Inactive()
                .Build();

            var partnerRepositoryMock = new Mock<IRepository<Partner>>();
            partnerRepositoryMock
                .Setup(x => x.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var controller = CreateController(partnerRepositoryMock);

            var limitDto = _fixture.Create<SetPartnerLimitDto>();

            // Act

            var result = await controller.SetPartnerPromoCodeLimitAsync(partner.Id, limitDto);

            // Assert

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}