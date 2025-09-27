using System;
using Xunit;
using System.Collections.Generic;

// 3.11 ~ 3.13 매개변수화 테스트
namespace unit_testing.Chapter3.Listing6
{
    public class DeliveryServiceTests
    {
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [Theory]
        public void Detects_an_invalid_delivery_date(int daysFromNow, bool expected)
        {
            DeliveryService sut = new DeliveryService();
            DateTime deliveryDate = DateTime.Now.AddDays(daysFromNow);
            Delivery delivery = new Delivery
            {
                Date = deliveryDate
            };

            bool actual = sut.IsDeliveryValid(delivery);

            Assert.Equal(expected, actual);
        }

        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [Theory]
        public void Detects_an_invalid_delivery_date2(int daysFromNow)
        {
            DeliveryService sut = new DeliveryService();
            DateTime deliveryDate = DateTime.Now.AddDays(daysFromNow);
            Delivery delivery = new Delivery
            {
                Date = deliveryDate
            };

            bool actual = sut.IsDeliveryValid(delivery);

            Assert.False(actual);
        }

        [Fact]
        public void The_soonest_delivery_date_is_two_days_from_now()
        {
            DeliveryService sut = new DeliveryService();
            DateTime deliveryDate = DateTime.Now.AddDays(2);
            Delivery delivery = new Delivery
            {
                Date = deliveryDate
            };

            bool isValid = sut.IsDeliveryValid(delivery);

            Assert.True(isValid);
        }

        [Theory]
        [MemberData(nameof(DataProvider))]
        public void Detects_an_invalid_delivery_date3(
            DateTime deliveryDate,
            bool expected)
        {
            DeliveryService sut = new DeliveryService();
            Delivery delivery = new Delivery
            {
                Date = deliveryDate
            };

            bool actual = sut.IsDeliveryValid(delivery);

            Assert.Equal(expected, actual);    
        }

        public static List<object[]> DataProvider()
        {
            return new List<object[]>
            {
                new object[] { DateTime.Now.AddDays(-1), false },
                new object[] { DateTime.Now, false },
                new object[] { DateTime.Now.AddDays(1), false },
                new object[] { DateTime.Now.AddDays(2), true }
            };
        }
    }

    public class Delivery
    {
        public DateTime Date { get; set; }
    }

    public class DeliveryService
    {
        public bool IsDeliveryValid(Delivery delivery)
        {
            return delivery.Date >= DateTime.Now.AddDays(1.999);
        }
    }
}