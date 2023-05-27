using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Protocol.Plugins;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Security.Claims;

namespace RefactorUnitTesting
{
    [TestClass]
    public class ProjectBLLUnitTest
    {
        private readonly ApplicationUser _primaryUser = new ApplicationUser { Id = "1" };
        private readonly ClaimsPrincipal _principal = new ClaimsPrincipal(new ClaimsIdentity("Test"));
        private readonly ProjectsBusinessLogic _projectBusinessLogic;
        private readonly List<Project> _projectData;
        private readonly List<UserProject> _userProjectData;
        private readonly List<Ticket> _ticketData;
        private readonly List<ApplicationUser> _userData;
        private Mock<UserManager<ApplicationUser>> _userManager;

        public ProjectBLLUnitTest()
        {            
            #region Project DB Setup
            _projectData = new List<Project>
            {
                new Project { Id = 1 },
                new Project { Id = 2},
                new Project { Id = 3 },
                new Project { Id = 4 }
            };

            IQueryable<Project> pQueryable = _projectData.AsQueryable();

            Mock<DbSet<Project>> mockProjectDbSet = new();
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(pQueryable.Provider);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(pQueryable.Expression);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(pQueryable.ElementType);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(pQueryable.GetEnumerator());

            mockProjectDbSet.Setup(m => m.Add(It.IsAny<Project>())).Callback<Project>(entity => _projectData.Add(entity));
            mockProjectDbSet.Setup(m => m.Remove(It.IsAny<Project>())).Callback<Project>(entity => _projectData.Remove(entity));
            mockProjectDbSet.Setup(m => m.Update(It.IsAny<Project>())).Callback<Project>(entity => _projectData[_projectData.FindIndex(t => t.Id == entity.Id)] = entity);
            #endregion

            #region Ticket DB Setup
            _ticketData = new List<Ticket>
            {
                new Ticket { Id = 1, ProjectId = 1, OwnerId = _primaryUser.Id },
                new Ticket { Id = 2, ProjectId = 1, OwnerId = "2" },
                new Ticket { Id = 3, ProjectId = 1, OwnerId = "3" },
                new Ticket { Id = 4, ProjectId = 1, OwnerId = "4", RequiredHours = 10 },
                new Ticket { Id = 5, ProjectId = 1, OwnerId = "5"}
            };

            IQueryable<Ticket> tQueryable = _ticketData.AsQueryable();

            Mock<DbSet<Ticket>> mockTicketDbSet = new();
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(tQueryable.Provider);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(tQueryable.Expression);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(tQueryable.ElementType);
            mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(tQueryable.GetEnumerator());
            #endregion

            #region User Project DB Setup
            _userProjectData = new List<UserProject>
            {
                new UserProject { ProjectId = 1, ApplicationUserId = _primaryUser.Id },
                new UserProject { ProjectId = 1, ApplicationUserId = "2" },
                new UserProject { ProjectId = 1, ApplicationUserId = "3" },
                new UserProject { ProjectId = 1, ApplicationUserId = "4" },
                new UserProject { ProjectId = 1, ApplicationUserId = "5" }
            };

            IQueryable<UserProject> upQueryable = _userProjectData.AsQueryable();

            Mock<DbSet<UserProject>> mockUserProjectDbSet = new();
            mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.Provider).Returns(upQueryable.Provider);
            mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.Expression).Returns(upQueryable.Expression);
            mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.ElementType).Returns(upQueryable.ElementType);
            mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.GetEnumerator()).Returns(upQueryable.GetEnumerator());

            mockUserProjectDbSet.Setup(m => m.Add(It.IsAny<UserProject>())).Callback<UserProject>(entity => _userProjectData.Add(entity));
            mockUserProjectDbSet.Setup(m => m.Remove(It.IsAny<UserProject>())).Callback<UserProject>(entity => _userProjectData.Remove(entity));
            mockUserProjectDbSet.Setup(m => m.Update(It.IsAny<UserProject>())).Callback<UserProject>(entity => _userProjectData[_userProjectData.FindIndex(t => t.Id == entity.Id)] = entity);

            #endregion

            #region User DB Setup
            _userData = new List<ApplicationUser>
            {
                _primaryUser,
                new ApplicationUser { Id = "2", UserName = "Gerald" },
                new ApplicationUser { Id = "3", UserName = "Tony" },
                new ApplicationUser { Id = "4", UserName = "Lester" },
                new ApplicationUser { Id = "5", UserName = "Franklin" },
            };

            IQueryable<ApplicationUser> uQueryable = _userData.AsQueryable();

            Mock<DbSet<ApplicationUser>> mockUserDbSet = new();
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(uQueryable.Provider);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(uQueryable.Expression);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(uQueryable.ElementType);
            mockUserDbSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(uQueryable.GetEnumerator());

            Mock<UserManager<ApplicationUser>> mockUserManager = new(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            mockUserManager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mockUserManager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mockUserManager.Setup(u => u.Users).Returns(mockUserDbSet.Object);
            mockUserManager.Setup(u => u.GetUserAsync(_principal)).ReturnsAsync(_primaryUser);
            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string userId) => _userData.FirstOrDefault(u => u.Id == userId));
            #endregion

            Mock<ApplicationDbContext> mockContext = new();
            mockContext.Setup(c => c.Tickets).Returns(mockTicketDbSet.Object);
            mockContext.Setup(c => c.Projects).Returns(mockProjectDbSet.Object);
            mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectDbSet.Object);

            _userManager = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
            _userManager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            _userManager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
            _userManager.Setup(u => u.Users).Returns(mockUserDbSet.Object);
            _userManager.Setup(u => u.GetUserAsync(_principal)).ReturnsAsync(_primaryUser);
            _userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => _userData.FirstOrDefault(u => u.Id == userId));



            _projectBusinessLogic = new(
                mockUserManager.Object,
                new TicketRepository(mockContext.Object),
                new ProjectsRepository(mockContext.Object),
                new UserProjectRepository(mockContext.Object)
            );
        }

        [TestMethod]
        [DataRow(1)]
        public void GetProjectDetails_ReturnsProjectAndCorrectId(int id)
        {
            Assert.IsTrue(_projectBusinessLogic.GetDetailsById(id) is Project p && p.Id == id);
        }

        [TestMethod]
        [DataRow(1)]
        public void GetProjectForDelete_ReturnsProjectAndCorrectId(int id)
        {
            Assert.IsTrue(_projectBusinessLogic.GetProjectForDelete(id) is Project p && p.Id == id);
        }

        [TestMethod]
        [DataRow(1)]
        public void DeleteProject_CorrectProjectIsDeleted(int id)
        {
            _projectBusinessLogic.DeleteProject(id);

            Assert.IsFalse(_projectData.Any(t => t.Id == id));
        }

        [TestMethod]
        public async Task CreateProjectAsync_ProjectIsCreated()
        {
            int newId = _projectData.Last().Id + 1;

            List<string> ustrings = new List<string>();
            Project newProj = new Project()
            {
                Id = newId,
            };

            ICollection<ApplicationUser> userstrings = _userData.Where(u => u.Id.Length > 0).ToList();
            foreach (var user in userstrings)
            {
                ustrings.Add(user.Id);
            }
            await _projectBusinessLogic.CreateProjectAsync(ustrings, newProj, _principal);

            Assert.IsTrue(_projectData.Any(t => t.Id == newId));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void ProjectExists_ProjectExistsReturnsTrue(int id)
        {
            Assert.IsTrue(_projectBusinessLogic.ProjectExists(id));
        }

        [TestMethod]
        [DataRow(0)]
        public void ProjectExists_NullProjectReturnsFalse(int id)
        {
            Assert.IsFalse(_projectBusinessLogic.ProjectExists(id));
        }

        [TestMethod]
        public void ProjectExists_ProjectDoesNotExist_ReturnsFalse()
        {
            int nonExistentProjectId = 10;
            Assert.IsFalse(_projectBusinessLogic.ProjectExists(nonExistentProjectId));
        }

        [TestMethod]
        public void GetAllDevelopersAsync_NullDeveloper_ThrowsException()
        {
            ApplicationUser nullDeveloper = null;
            _userManager.Setup(u => u.GetUsersInRoleAsync("Developer")).ReturnsAsync(new List<ApplicationUser> { nullDeveloper });



            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
            {
                var task = _projectBusinessLogic.GetAllDevelopersAsync();
                task.Wait();
            });

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType(exception.InnerException, typeof(Exception));
            Assert.AreEqual("Object reference not set to an instance of an object.", exception.InnerException.Message);
        }
    }
}