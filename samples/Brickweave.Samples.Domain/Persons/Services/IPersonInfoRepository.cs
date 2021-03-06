﻿using System.Threading.Tasks;
using Brickweave.Samples.Domain.Persons.Models;

namespace Brickweave.Samples.Domain.Persons.Services
{
    public interface IPersonInfoRepository
    {
        Task<PersonInfo> GetPersonInfoAsync(PersonId personId);
    }
}