
// 예제 5.5 구현 세부 사항을 유출하는 User 클래스
namespace unit_testing.Chapter5.Listing6
{
    public class User
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = NormalizeName(value);
        }
        

        private string NormalizeName(string name) // ✅ private로 변경
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
            user.Name = newName; // ✅ 캡슐화
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