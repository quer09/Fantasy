﻿using Fantasy.Backend.Data;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entity;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _entity = context.Set<T>();
    }

    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _context.Add(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExeptionActionResponse();
        }
        catch (Exception exeption)
        {
            return ExeptionActionResponse(exeption);
        }
    }

    public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        _entity.Remove(row);
        try
        {
            await _context.SaveChangesAsync();

            return new ActionResponse<T>
            {
                WasSuccess = true
            };
        }
        catch (Exception)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "ERR002"
            };
        }
    }

    public virtual async Task<ActionResponse<T>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null)
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }
        return new ActionResponse<T>
        {
            WasSuccess = true,
            Result = row
        };
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync()
    {
        return new ActionResponse<IEnumerable<T>>
        {
            WasSuccess = true,
            Result = await _entity.ToListAsync()
        };
    }

    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {
        _context.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExeptionActionResponse();
        }
        catch (Exception exeption)
        {
            return ExeptionActionResponse(exeption);
        }
    }

    private ActionResponse<T> ExeptionActionResponse(Exception exeption)
    {
        return new ActionResponse<T>
        {
            WasSuccess = false,
            Message = exeption.Message
        };
    }

    private ActionResponse<T> DbUpdateExeptionActionResponse()
    {
        return new ActionResponse<T>
        {
            WasSuccess = false,
            Message = "ERR003"
        };
    }
}