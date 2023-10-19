using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Drawing;
using VolgaIT.Model.Entities;
using VolgaIT.Model.Model;

namespace VolgaIT.OtherClasses
{
    //CEAM - Converter Entity And Model
    public class CEAM
    {

        public static UserEntity UserModelToEntity(User user)
        {
            UserEntity userEntity = new UserEntity() 
            { 
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                Username = user.Username,
                Password = user.Password,
                Balance = user.Balance
            };

            return userEntity;

        }
        public static TransportEntity TransportModelToEntity(Model.Model.Transport transport)
        {
            TransportEntity transportEntity = new TransportEntity()
            {
                Id = transport.Id,
                CanBeRented = transport.CanBeRented,
                TransportType = transport.TransportType,
                Model = transport.Model,
                Color = transport.Color,
                Identifier = transport.Identifier,
                Description = transport.Description,
                Latitude = transport.Latitude,
                Longitude = transport.Longitude,
                MinutePrice = transport.MinutePrice,
                DayPrice = transport.DayPrice
            };

            return transportEntity;
        }
        public static RentEntity RentModelToEntity(Rent rent)
        {
            RentEntity rentEntity = new RentEntity()
            {
                Id = rent.Id,
                TransportId = rent.TransportId,
                UserId = rent.UserId,
                TimeStart = rent.TimeStart,
                TimeEnd = rent.TimeEnd,
                PriceOfUnit = rent.PriceOfUnit,
                PriceType = rent.PriceType,
                FinalPrice = rent.FinalPrice
            };

            return rentEntity;
        }

        public static User UserEntityToModel(UserEntity userEntity)
        {
            User user = new User()
            {
                Id = userEntity.Id,
                IsAdmin = userEntity.IsAdmin,
                Username = userEntity.Username,
                Password = userEntity.Password,
                Balance = userEntity.Balance
            };

            return user;

        }
        public static Model.Model.Transport TransportEntityToModel(TransportEntity transportEntity)
        {
            Model.Model.Transport transport = new Model.Model.Transport()
            {
                Id = transportEntity.Id,
                OwnerId = transportEntity.OwnerId,
                CanBeRented = transportEntity.CanBeRented,
                TransportType = transportEntity.TransportType,
                Model = transportEntity.Model,
                Color = transportEntity.Color,
                Identifier = transportEntity.Identifier,
                Description = transportEntity.Description,
                Latitude = transportEntity.Latitude,
                Longitude = transportEntity.Longitude,
                MinutePrice = transportEntity.MinutePrice,
                DayPrice = transportEntity.DayPrice
            };

            return transport;
        }
        public static Rent RentEntityToModel(RentEntity rentEntity)
        {
            Rent rent = new Rent()
            {
                Id = rentEntity.Id,
                TransportId = rentEntity.TransportId,
                UserId = rentEntity.UserId,
                TimeStart = rentEntity.TimeStart,
                TimeEnd = rentEntity.TimeEnd,
                PriceOfUnit = rentEntity.PriceOfUnit,
                PriceType = rentEntity.PriceType,
                FinalPrice = rentEntity.FinalPrice
            };

            return rent;
        }

    }
}
