using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http;

/// <summary>
///     Base controller class providing common result handling methods. Optimized for performance with cached static instances and efficient memory usage.
/// </summary>
[ApiController]
public abstract class ControllerRepositoryBase : ControllerBase {

    /// <summary>
    ///     Gets a result indicating a forbidden failure.
    /// </summary>
    protected static ITnTResult FailureForbidden => Failure(CachedForbiddenException);

    /// <summary>
    ///     Gets a result indicating an internal server error.
    /// </summary>
    protected static ITnTResult FailureInternalServerError => HttpTnTResult.InternalServerError();

    /// <summary>
    ///     Gets a result indicating an unauthorized failure.
    /// </summary>
    protected static ITnTResult FailureUnauthorized => Failure(CachedUnauthorizedException);

    /// <summary>
    ///     Gets a result indicating a successful operation.
    /// </summary>
    protected static ITnTResult Successful => HttpTnTResult.Successful;

    /// <summary>
    ///     Gets a result indicating a successfully accepted operation.
    /// </summary>
    protected static ITnTResult SuccessfullyAccepted => HttpTnTResult.Accepted();

    /// <summary>
    ///     Gets a result indicating a successfully created operation.
    /// </summary>
    protected static ITnTResult SuccessfullyCreated => HttpTnTResult.Created();

    /// <summary>
    ///     Gets the user ID from the current claims principal. Returns null if the user is not authenticated or the claim is not present.
    /// </summary>
    protected virtual string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    private static readonly ForbiddenException CachedForbiddenException = new();

    private static readonly UnauthorizedAccessException CachedUnauthorizedException = new();

    /// <summary>
    ///     Creates a conflict result with the specified message.
    /// </summary>
    /// <param name="message">The conflict message.</param>
    /// <returns>The conflict result.</returns>
    protected static ITnTResult Conflict(string message) => HttpTnTResult.Failure(message, TypedResults.Conflict(message));

    /// <summary>
    ///     Creates a conflict result with the specified message and success type.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="message">The conflict message.</param>
    /// <returns>The conflict result.</returns>
    protected static ITnTResult<TSuccess> Conflict<TSuccess>(string message) => HttpTnTResult<TSuccess>.Failure(message, TypedResults.Conflict(message));

    /// <summary>
    ///     Creates a created result with the specified value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>The created result.</returns>
    protected static ITnTResult<TSuccess> Created<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Created(value);

    /// <summary>
    ///     Creates a custom error result with the specified exception and result.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="result">   The result to return.</param>
    /// <returns>The custom error result.</returns>
    protected static ITnTResult CustomError(Exception exception, IResult result) => HttpTnTResult.CustomError(exception, result);

    /// <summary>
    ///     Creates a custom error result with the specified exception, result, and success type.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="result">   The result to return.</param>
    /// <returns>The custom error result.</returns>
    protected static ITnTResult<TSuccess> CustomError<TSuccess>(Exception exception, IResult result) => HttpTnTResult<TSuccess>.CustomError(exception, result);

    /// <summary>
    ///     Creates a custom result with the specified result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    /// <returns>The custom result.</returns>
    protected static ITnTResult CustomResult(IResult result) => HttpTnTResult.CustomResult(result);

    /// <summary>
    ///     Creates a custom result with the specified value and result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="value"> The success value.</param>
    /// <param name="result">The result to return.</param>
    /// <returns>The custom result.</returns>
    protected static ITnTResult<TSuccess> CustomResult<TSuccess>(TSuccess value, IResult result) => HttpTnTResult<TSuccess>.CustomResult(value, result);

    /// <summary>
    ///     Creates a failure result with the specified message. Optimized to reuse exception instances when possible.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>The failure result.</returns>
    protected static ITnTResult Failure(string message) => Failure(new Exception(message));

    /// <summary>
    ///     Creates a failure result with the specified exception.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>The failure result.</returns>
    protected static ITnTResult Failure(Exception ex) => HttpTnTResult.Failure(ex);

    /// <summary>
    ///     Creates a failure result with the specified message and success type. Optimized to reuse exception instances when possible.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="message">The failure message.</param>
    /// <returns>The failure result.</returns>
    protected static ITnTResult<TSuccess> Failure<TSuccess>(string message) => Failure<TSuccess>(new Exception(message));

    /// <summary>
    ///     Creates a failure result with the specified exception and success type.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="ex">The exception.</param>
    /// <returns>The failure result.</returns>
    protected static ITnTResult<TSuccess> Failure<TSuccess>(Exception ex) => HttpTnTResult<TSuccess>.Failure(ex);

    /// <summary>
    ///     Creates a forbidden failure result with the specified success type. Uses cached exception instance for better performance.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <returns>The forbidden failure result.</returns>
    protected static ITnTResult<TSuccess> Forbid<TSuccess>() => Failure<TSuccess>(CachedForbiddenException);



    /// <summary>
    ///     Creates a not found result for the specified entity type and key.
    /// </summary>
    /// <typeparam name="EntityType">The type of the entity.</typeparam>
    /// <param name="key">The key of the entity.</param>
    /// <returns>The not found result.</returns>
    protected static ITnTResult NotFound<EntityType>(object key) => HttpTnTResult.Failure(new NotFoundException(typeof(EntityType), key));

    /// <summary>
    ///     Creates a not found result for the specified entity type, key, and success type.
    /// </summary>
    /// <typeparam name="EntityType">The type of the entity.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="key">The key of the entity.</param>
    /// <returns>The not found result.</returns>
    protected static ITnTResult<TSuccess> NotFound<EntityType, TSuccess>(object key) => HttpTnTResult<TSuccess>.Failure(new NotFoundException(typeof(EntityType), key));

    /// <summary>
    ///     Creates a redirect result with the specified URI, permanence, and method preservation.
    /// </summary>
    /// <param name="uri">           The URI to redirect to.</param>
    /// <param name="permanent">     Indicates whether the redirection is permanent (default: false).</param>
    /// <param name="preserveMethod">Indicates whether to preserve the HTTP method (default: false).</param>
    /// <returns>The redirect result.</returns>
    protected static ITnTResult Redirect(Uri uri, bool permanent = false, bool preserveMethod = false) => HttpTnTResult.Redirect(uri, permanent, preserveMethod);

    /// <summary>
    ///     Creates a redirect result with the specified value, URI, permanence, and method preservation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="value">         The success value.</param>
    /// <param name="uri">           The URI to redirect to.</param>
    /// <param name="permanent">     Indicates whether the redirection is permanent (default: false).</param>
    /// <param name="preserveMethod">Indicates whether to preserve the HTTP method (default: false).</param>
    /// <returns>The redirect result.</returns>
    protected static ITnTResult<TSuccess> Redirect<TSuccess>(TSuccess value, Uri uri, bool permanent = false, bool preserveMethod = false) => HttpTnTResult<TSuccess>.Redirect(value, uri, permanent, preserveMethod);

    /// <summary>
    ///     Creates a success result with the specified value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>The success result.</returns>
    protected static ITnTResult<TSuccess> Success<TSuccess>(TSuccess value) => HttpTnTResult<TSuccess>.Success(value);

    /// <summary>
    ///     Creates an unauthorized failure result with the specified success type. Uses cached exception instance for better performance.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <returns>The unauthorized failure result.</returns>
    protected static ITnTResult<TSuccess> Unauthorized<TSuccess>() => Failure<TSuccess>(CachedUnauthorizedException);

#if NET9_0_OR_GREATER
    /// <summary>
    ///     Creates an internal server error result with the specified success type and optional message.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="message">Optional error message.</param>
    /// <returns>The internal server error result.</returns>
    protected static ITnTResult<TSuccess> InternalServerError<TSuccess>(string? message = null) => HttpTnTResult<TSuccess>.InternalServerError(message);

    /// <summary>
    ///     Creates an internal server error result with the specified success type and problem details.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="problemDetails">The problem details to include in the error response.</param>
    /// <returns>The internal server error result.</returns>
    protected static ITnTResult<TSuccess> InternalServerError<TSuccess>(ProblemDetails? problemDetails) => HttpTnTResult<TSuccess>.InternalServerError(problemDetails);
#else
    /// <summary>
    ///     Creates an internal server error result with the specified success type.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <returns>The internal server error result.</returns>
    protected static ITnTResult<TSuccess> InternalServerError<TSuccess>() => HttpTnTResult<TSuccess>.InternalServerError();
#endif
}