using Discord;

namespace Joshtron
{
    public static class LeeDetector
    {
        public static bool IsLee(User user)
        {
            if (user.Name.ToLower().Contains("odyssic") ||
                user.Name.ToLower().Contains("oddington"))
            {
                return true;
            }

            return false;
        }
    }
}
