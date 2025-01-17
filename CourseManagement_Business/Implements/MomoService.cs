using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebCourseManagement2_Business.Interfaces;
using WebCourseManagement2_Models.DataContexts;
using WebCourseManagement2_Models.ResponseModels;

namespace WebCourseManagement2_Business.Implements
{
    public class MomoService : IMomoService
    {
        private readonly AppDbContext _context;
        public MomoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePaymentAsync(int hoaDonId)
        {
            
        }

        public MomoExcuteResponseModel PaymentExcuteAsync(IQueryCollection collection)
        {
            throw new NotImplementedException();
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
