using System;
using System.Collections.Generic;

namespace VehicleTracking.Application.Models
{
    /// <summary>
    /// API yanıtları için standart model
    /// </summary>
    /// <typeparam name="T">Yanıt veri tipi</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// İşlemin başarılı olup olmadığı
        /// </summary>
        public bool Success { get; set; } = true;
        
        /// <summary>
        /// İşlem başarısızsa hata mesajı
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Yanıt verisi
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Varsa detaylı hata mesajları listesi
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>
        /// Yanıtın oluşturulma zamanı
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Başarılı bir yanıt oluşturur
        /// </summary>
        /// <param name="data">Yanıt verisi</param>
        /// <param name="message">Opsiyonel mesaj</param>
        /// <returns>Başarılı API yanıtı</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }
        
        /// <summary>
        /// Başarısız bir yanıt oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <param name="errors">Detaylı hata mesajları listesi</param>
        /// <returns>Başarısız API yanıtı</returns>
        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
    
    /// <summary>
    /// Veri içermeyen API yanıtları için basitleştirilmiş model
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Başarılı bir yanıt oluşturur (veri içermez, sadece bilgi mesajı)
        /// </summary>
        /// <param name="message">Bilgi mesajı</param>
        /// <returns>Başarılı API yanıtı</returns>
        public static ApiResponse SuccessResponse(string message = "İşlem başarıyla tamamlandı")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = null
            };
        }
        
        /// <summary>
        /// Başarısız bir yanıt oluşturur
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <param name="errors">Detaylı hata mesajları listesi</param>
        /// <returns>Başarısız API yanıtı</returns>
        public static new ApiResponse ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
} 