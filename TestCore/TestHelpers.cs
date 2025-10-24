using Grocery.Core.Helpers;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Grocery.Core.Interfaces.Repositories;
using Moq;

namespace TestCore
{
    public class TestHelpers
    {
        private GroceryListService _service;
        private Mock<IGroceryListRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            // Mock database lists voor GroceryListService tests
            var dbLists = new List<GroceryList>
            {
                new GroceryList(1, "Boodschappen familieweekend", new DateOnly(2024, 12, 14), "#FF6A00", 1),
                new GroceryList(2, "Kerstboodschappen", new DateOnly(2024, 12, 7), "#626262", 1),
                new GroceryList(3, "Weekend boodschappen", new DateOnly(2024, 11, 30), "#003300", 1)
            };

            _mockRepository = new Mock<IGroceryListRepository>();
            _mockRepository.Setup(r => r.GetAll()).Returns(dbLists);

            _service = new GroceryListService(_mockRepository.Object);
        }

        //Password Helper Tests
        //Happy flow
        [Test]
        public void TestPasswordHelperReturnsTrue()
        {
            string password = "user3";
            string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        public void TestPasswordHelperReturnsTrue(string password, string passwordHash)
        {
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }


        //Unhappy flow
        [Test]
        public void TestPasswordHelperReturnsFalse()
        {
            string password = "user3";
            string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ";
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ")]
        public void TestPasswordHelperReturnsFalse(string password, string passwordHash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }


        //GroceryList Service Tests
        // Test 1: Nieuwe lijst toevoegen krijgt negatief ID
        [Test]
        public void TestAddGroceryList_ReturnsNegativeId()
        {
            var newList = new GroceryList(0, "Test lijst", DateOnly.FromDateTime(DateTime.Today), "#FF0000", 1);

            var result = _service.Add(newList);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.LessThan(0));
            Assert.That(result.Name, Is.EqualTo("Test lijst"));
        }


        // Test 2: GetAll combineert database en nieuwe lijsten
        [Test]
        public void TestGetAll_CombinesDatabaseAndTemporaryLists()
        {
            _service.Add(new GroceryList(0, "Temp lijst 1", DateOnly.FromDateTime(DateTime.Today), "#FF0000", 1));
            _service.Add(new GroceryList(0, "Temp lijst 2", DateOnly.FromDateTime(DateTime.Today), "#00FF00", 1));

            var allLists = _service.GetAll();

            Assert.That(allLists.Count, Is.EqualTo(5)); 
        }


        // Test 3: Get haalt temporary lijst op met negatief ID
        [Test]
        public void TestGet_ReturnsTemporaryListByNegativeId()
        {
            var addedList = _service.Add(new GroceryList(0, "Test lijst", DateOnly.FromDateTime(DateTime.Today), "#FF0000", 1));

            var retrieved = _service.Get(addedList.Id);

            Assert.IsNotNull(retrieved);
            Assert.That(retrieved.Id, Is.EqualTo(addedList.Id));
            Assert.That(retrieved.Name, Is.EqualTo("Test lijst"));
        }
    }
}