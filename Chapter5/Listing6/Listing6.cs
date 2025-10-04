
// 예제 5.5 구현 세부 사항을 유출하는 User 클래스
namespace unit_testing.Chapter5.Listing5
{
    public class User
    {
        public string Name { get; set; }

        public string NormalizeName(string name)
        {
            string result = (name ?? "").Trim();

            if (result.Length > 50)
                return result.Substring(0, 50);

            return result;
        }
    }

    public class UserController
    {
        public void RenameUser(int userId, string newName)
        {
            User user = GetUserFromDatabase(userId);

            string normalizeName = user.NormalizeName(newName); // 💩
            user.Name = normalizeName;

            SaveUserToDatabase(user);
        }

        private void SaveUserToDatabase(User user)
        {
            // do something;
        }

        private User GetUserFromDatabase(int userId)
        {
            return new User();
        }
    }
}