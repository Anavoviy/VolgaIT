﻿namespace VolgaIT.Model.Interfaces
{
    public interface IEntity
    {
        long Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
