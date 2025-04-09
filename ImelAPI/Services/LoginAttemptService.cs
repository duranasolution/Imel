using Microsoft.Extensions.Caching.Memory;

namespace ImelAPI.Services
{
    public class LoginAttemptService
    {
        private readonly IMemoryCache _memoryCache;
        private const int MaxFailedAttempts = 5;  
        private const int LockoutDurationInMinutes = 15;

        public LoginAttemptService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool IsAccountLocked(string email)
        {
            var failedAttempts = _memoryCache.Get<int>($"{email}_FailedAttempts");
            var lockoutTime = _memoryCache.Get<DateTime?>($"{email}_LockoutTime");

            if (lockoutTime.HasValue && lockoutTime.Value > DateTime.Now)
            {
                return true;  
            }

            return false; 
        }

        public void RecordFailedAttempt(string email)
        {
            var failedAttempts = _memoryCache.Get<int>($"{email}_FailedAttempts");
            failedAttempts++;
            _memoryCache.Set($"{email}_FailedAttempts", failedAttempts, TimeSpan.FromMinutes(LockoutDurationInMinutes));

            if (failedAttempts >= MaxFailedAttempts)
            {
                _memoryCache.Set($"{email}_LockoutTime", DateTime.Now.AddMinutes(LockoutDurationInMinutes));
            }
        }

        public void ResetFailedAttempts(string email)
        {
            _memoryCache.Remove($"{email}_FailedAttempts");
            _memoryCache.Remove($"{email}_LockoutTime");
        }
    }
}
