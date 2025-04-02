using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.Models;

namespace VehicleTracking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Başarılı bir yanıt oluşturur
        /// </summary>
        /// <typeparam name="T">Veri tipi</typeparam>
        /// <param name="data">Yanıt verisi</param>
        /// <param name="message">Bilgi mesajı (opsiyonel)</param>
        /// <returns>200 OK yanıtı</returns>
        protected IActionResult Ok<T>(T data, string message = "")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return base.Ok(response);
        }
        
        /// <summary>
        /// Veri içermeyen başarılı bir yanıt oluşturur
        /// </summary>
        /// <param name="message">Bilgi mesajı</param>
        /// <returns>200 OK yanıtı</returns>
        protected IActionResult Ok(string message = "İşlem başarıyla tamamlandı")
        {
            var response = ApiResponse.SuccessResponse(message);
            return base.Ok(response);
        }
        
        /// <summary>
        /// Yeni oluşturulan kayıt için yanıt oluşturur
        /// </summary>
        /// <typeparam name="T">Veri tipi</typeparam>
        /// <param name="data">Yanıt verisi</param>
        /// <param name="message">Bilgi mesajı (opsiyonel)</param>
        /// <returns>201 Created yanıtı</returns>
        protected IActionResult Created<T>(T data, string message = "Kayıt başarıyla oluşturuldu")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return StatusCode(201, response);
        }
        
        /// <summary>
        /// Yeni oluşturulan kayıt için ilgili action'a yönlendirme yapan yanıt oluşturur
        /// </summary>
        /// <typeparam name="T">Veri tipi</typeparam>
        /// <param name="actionName">Action adı</param>
        /// <param name="routeValues">Route değerleri</param>
        /// <param name="data">Yanıt verisi</param>
        /// <param name="message">Bilgi mesajı (opsiyonel)</param>
        /// <returns>201 Created yanıtı</returns>
        protected IActionResult CreatedAtAction<T>(string actionName, object routeValues, T data, string message = "Kayıt başarıyla oluşturuldu")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return base.CreatedAtAction(actionName, routeValues, response);
        }
        
        /// <summary>
        /// Hata yanıtı oluşturur
        /// </summary>
        /// <typeparam name="T">Veri tipi</typeparam>
        /// <param name="message">Hata mesajı</param>
        /// <param name="errors">Detaylı hata mesajları (opsiyonel)</param>
        /// <returns>400 Bad Request yanıtı</returns>
        protected IActionResult BadRequest<T>(string message, List<string> errors = null)
        {
            var response = ApiResponse<T>.ErrorResponse(message, errors);
            return new BadRequestObjectResult(response);
        }
        
        /// <summary>
        /// Veri tipi belirtmeden hata yanıtı oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <param name="errors">Detaylı hata mesajları (opsiyonel)</param>
        /// <returns>400 Bad Request yanıtı</returns>
        protected IActionResult BadRequest(string message, List<string> errors = null)
        {
            var response = ApiResponse.ErrorResponse(message, errors);
            return new BadRequestObjectResult(response);
        }
        
        /// <summary>
        /// Kayıt bulunamadı yanıtı oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <returns>404 Not Found yanıtı</returns>
        protected IActionResult NotFound(string message = "Kayıt bulunamadı")
        {
            var response = ApiResponse.ErrorResponse(message);
            return new NotFoundObjectResult(response);
        }
        
        /// <summary>
        /// Yetkisiz erişim yanıtı oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <returns>401 Unauthorized yanıtı</returns>
        protected IActionResult Unauthorized(string message = "Bu işlem için yetkiniz bulunmamaktadır")
        {
            var response = ApiResponse.ErrorResponse(message);
            return new UnauthorizedObjectResult(response);
        }
        
        /// <summary>
        /// Erişim reddedildi yanıtı oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <returns>403 Forbidden yanıtı</returns>
        protected IActionResult Forbidden(string message = "Bu işlem için erişim reddedildi")
        {
            var response = ApiResponse.ErrorResponse(message);
            return StatusCode(403, response);
        }
        
        /// <summary>
        /// Sunucu hatası yanıtı oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <returns>500 Internal Server Error yanıtı</returns>
        protected IActionResult ServerError(string message = "İşlem sırasında bir hata oluştu")
        {
            var response = ApiResponse.ErrorResponse(message);
            return StatusCode(500, response);
        }
    }
} 