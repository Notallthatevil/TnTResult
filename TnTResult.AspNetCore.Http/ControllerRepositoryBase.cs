using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http;
[ApiController]
public abstract class ControllerRepositoryBase : ControllerBase {
    protected static ITnTResult Successful => HttpTnTResult.Successful;
    protected static ITnTResult Failure(string message) => Failure(new Exception(message));
    protected static ITnTResult Failure(Exception ex) => HttpTnTResult.Failure(ex);
    protected static ITnTResult<TSuccess> Failure<TSuccess>(string message) => Failure<TSuccess>(new Exception(message));
    protected static ITnTResult<TSuccess> Failure<TSuccess>(Exception ex) => HttpTnTResult<TSuccess>.Failure(ex);
    protected static ITnTResult NotFound<EntityType>(object key) => HttpTnTResult.Failure(new NotFoundException(typeof(EntityType), key));
    protected static ITnTResult<TSuccess> NotFound<EntityType, TSuccess>(object key) => HttpTnTResult<TSuccess>.Failure(new NotFoundException(typeof(EntityType), key));
    protected static ITnTResult<TSuccess> Success<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Success(value);

    new protected static ITnTResult Unauthorized() => Failure(new UnauthorizedAccessException());
    protected static ITnTResult<TSuccess> Unauthorized<TSuccess>() => Failure<TSuccess>(new UnauthorizedAccessException());
    new protected static ITnTResult Created() => HttpTnTResult.Created;
    protected static ITnTResult<TSuccess> Created<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Created(value);
}
