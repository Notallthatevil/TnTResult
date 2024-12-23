﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http;

[ApiController]
public abstract class ControllerRepositoryBase : ControllerBase {
    protected static ITnTResult FailureUnauthorized => Failure(new UnauthorizedAccessException());
    protected static ITnTResult Successful => HttpTnTResult.Successful;
    protected static ITnTResult SuccessfullyCreated => HttpTnTResult.Created();
    protected static ITnTResult SuccessfullyAccepted => HttpTnTResult.Accepted();
    protected virtual string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    protected static ITnTResult Conflict(string message) => HttpTnTResult.Failure(message, TypedResults.Conflict(message));
    protected static ITnTResult<TSuccess> Conflict<TSuccess>(string message) => HttpTnTResult<TSuccess>.Failure(message, TypedResults.Conflict(message));

    protected static ITnTResult<TSuccess> Created<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Created(value);

    protected static ITnTResult Failure(string message) => Failure(new Exception(message));

    protected static ITnTResult Failure(Exception ex) => HttpTnTResult.Failure(ex);

    protected static ITnTResult<TSuccess> Failure<TSuccess>(string message) => Failure<TSuccess>(new Exception(message));

    protected static ITnTResult<TSuccess> Failure<TSuccess>(Exception ex) => HttpTnTResult<TSuccess>.Failure(ex);

    protected static ITnTResult NotFound<EntityType>(object key) => HttpTnTResult.Failure(new NotFoundException(typeof(EntityType), key));

    protected static ITnTResult<TSuccess> NotFound<EntityType, TSuccess>(object key) => HttpTnTResult<TSuccess>.Failure(new NotFoundException(typeof(EntityType), key));

    protected static ITnTResult<TSuccess> Success<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Success(value);

    protected static ITnTResult<TSuccess> Unauthorized<TSuccess>() => Failure<TSuccess>(new UnauthorizedAccessException());
}