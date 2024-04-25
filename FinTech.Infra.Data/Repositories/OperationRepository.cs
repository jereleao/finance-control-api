using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using FinTech.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace FinTech.Infra.Data.Repositories;

public class OperationRepository(ApplicationDbContext context) : BaseRepository<Operation>(context), IOperationRepository
{
    
}
