using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Security.Claims;

namespace RefactorUnitTesting
{
    [TestClass]
    public class TicketBLLUnitTest
    {
        private readonly ApplicationUser _primaryUser = new() { Id = "1" };
        private readonly ClaimsPrincipal _principal = new(new ClaimsIdentity("Test"));

        private readonly TicketBusinessLogic _ticketBusinessLogic;

        private readonly List<Ticket> _ticketData;
        private readonly List<Project> _projectData;
        private readonly List<Comment> _commentData;
        private readonly List<UserProject> _userProjectData;
        private readonly List<TicketWatcher> _ticketWatcherData;
        private readonly List<ApplicationUser> _userData;

        public TicketBLLUnitTest()
        {
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

            mockTicketDbSet.Setup(m => m.Add(It.IsAny<Ticket>())).Callback<Ticket>(entity => _ticketData.Add(entity));
            mockTicketDbSet.Setup(m => m.Remove(It.IsAny<Ticket>())).Callback<Ticket>(entity => _ticketData.Remove(entity));
            mockTicketDbSet.Setup(m => m.Update(It.IsAny<Ticket>())).Callback<Ticket>(entity => _ticketData[_ticketData.FindIndex(t => t.Id == entity.Id)] = entity);
            #endregion

            #region Project DB Setup
            _projectData = new List<Project>
            {
                new Project { Id = 1 }
            };

            IQueryable<Project> pQueryable = _projectData.AsQueryable();

            Mock<DbSet<Project>> mockProjectDbSet = new();
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(pQueryable.Provider);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(pQueryable.Expression);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(pQueryable.ElementType);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(pQueryable.GetEnumerator());
            #endregion

            #region Comment DB Setup
            _commentData = new List<Comment>
            {
            };

            IQueryable<Comment> cQueryable = _commentData.AsQueryable();

            Mock<DbSet<Comment>> mockCommentDbSet = new();
            mockCommentDbSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(cQueryable.Provider);
            mockCommentDbSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(cQueryable.Expression);
            mockCommentDbSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(cQueryable.ElementType);
            mockCommentDbSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(cQueryable.GetEnumerator());

            mockCommentDbSet.Setup(m => m.Add(It.IsAny<Comment>())).Callback<Comment>(entity => _commentData.Add(entity));
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
            #endregion

            #region Ticket Watcher DB Setup
            _ticketWatcherData = new List<TicketWatcher>
            {
                new TicketWatcher { TicketId = 1, WatcherId = _primaryUser.Id },
                new TicketWatcher { TicketId = 2, WatcherId = "2" },
                new TicketWatcher { TicketId = 3, WatcherId = "3" },
                new TicketWatcher { TicketId = 4, WatcherId = "4" },
                new TicketWatcher { TicketId = 5, WatcherId = "5" },
            };

            IQueryable<TicketWatcher> twQueryable = _ticketWatcherData.AsQueryable();

            Mock<DbSet<TicketWatcher>> mockTicketWatcherDbSet = new();
            mockTicketWatcherDbSet.As<IQueryable<TicketWatcher>>().Setup(m => m.Provider).Returns(twQueryable.Provider);
            mockTicketWatcherDbSet.As<IQueryable<TicketWatcher>>().Setup(m => m.Expression).Returns(twQueryable.Expression);
            mockTicketWatcherDbSet.As<IQueryable<TicketWatcher>>().Setup(m => m.ElementType).Returns(twQueryable.ElementType);
            mockTicketWatcherDbSet.As<IQueryable<TicketWatcher>>().Setup(m => m.GetEnumerator()).Returns(twQueryable.GetEnumerator());

            mockTicketWatcherDbSet.Setup(m => m.Add(It.IsAny<TicketWatcher>())).Callback<TicketWatcher>(entity => _ticketWatcherData.Add(entity));
            mockTicketWatcherDbSet.Setup(m => m.Remove(It.IsAny<TicketWatcher>())).Callback<TicketWatcher>(entity => _ticketWatcherData.Remove(entity));
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
            mockContext.Setup(c => c.Comments).Returns(mockCommentDbSet.Object);
            mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectDbSet.Object);
            mockContext.Setup(c => c.TicketWatchers).Returns(mockTicketWatcherDbSet.Object);

            _ticketBusinessLogic = new(
                mockUserManager.Object,
                new TicketRepository(mockContext.Object),
                new ProjectsRepository(mockContext.Object),
                new CommentRepository(mockContext.Object),
                new UserProjectRepository(mockContext.Object),
                new TicketWatcherRepository(mockContext.Object)
            );
        }

        [TestMethod]
        public async Task GetAllTicketsAsync_CountMatches()
        {
            Assert.AreEqual(_ticketData.Count, (await _ticketBusinessLogic.GetAllTicketsAsync()).Count());
        }

        [TestMethod]
        [DataRow(4)]
        public async Task GetTicketDetailsAsync_ViewModelIdMatchesIdArgument(int id)
        {
            Assert.IsTrue((await _ticketBusinessLogic.GetTicketDetailsAsync(id)).Item.Id == id);
        }

        [TestMethod]
        [DataRow(0)]
        public async Task GetTicketDetailsAsync_ThrowsNullReferenceExceptionWhenTicketNull(int id)
        {
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _ticketBusinessLogic.GetTicketDetailsAsync(id));
        }

        [TestMethod]
        [DataRow(1)]
        public async Task GetTicketForCreationAsync_ViewModelProjectIdMatchesProjectIdArgument(int projectId)
        {
            Assert.IsTrue((await _ticketBusinessLogic.GetTicketForCreationAsync(projectId)).Item.ProjectId == projectId);
        }

        [TestMethod]
        public async Task GetTicketForCreationAsync_ThrowsArgumentNullExceptionWhenIdNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await _ticketBusinessLogic.GetTicketForCreationAsync(null));
        }

        [TestMethod]
        public async Task CreateTicketAsync_TicketIsCreated()
        {
            int newId = _ticketData.Last().Id + 1;
            await _ticketBusinessLogic.CreateTicketAsync(new Ticket { Id = newId, ProjectId = 1, OwnerId = _primaryUser.Id });

            Assert.IsTrue(_ticketData.Any(t => t.Id == newId));
        }

        [TestMethod]
        public async Task CreateTicketAsync_ThrowsNullReferenceExceptionWhenProjectNull()
        {
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _ticketBusinessLogic.CreateTicketAsync(
                    new Ticket { Id = _ticketData.Last().Id + 1, OwnerId = _primaryUser.Id }));
        }

        [TestMethod]
        [DataRow(1)]
        public async Task GetTicketForEditingAsync_ViewModelTicketIdMatchesIdArgument(int id)
        {
            Assert.IsTrue((await _ticketBusinessLogic.GetTicketForEditingAsync(id)).Item.Id == id);
        }

        [TestMethod]
        [DataRow(0)]
        public async Task GetTicketForEditingAsync_ThrowsNullReferenceExceptionWhenTicketNull(int id)
        {
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _ticketBusinessLogic.GetTicketForEditingAsync(id));
        }

        [TestMethod]
        [DataRow(1, "EditTestBody")]
        [DataRow(2, "EditTestBodyButLonger")]
        public void EditTicket_ChangesAreSaved(int id, string body)
        {
            _ticketBusinessLogic.EditTicket(id, new Ticket { Id = id, Body = body, OwnerId = _primaryUser.Id });

            Ticket ticket = _ticketData.First(t => t.Id == id);

            Assert.AreEqual(body, ticket.Body);
        }

        [TestMethod]
        [DataRow(3, "6")]
        [DataRow(5, "0")]
        public void EditTicket_ThrowsArgumentExceptionWhenOwnerNull(int id, string ownerId)
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ticketBusinessLogic.EditTicket(id, new Ticket { Id = id, OwnerId = ownerId }));
        }

        [TestMethod]
        [DataRow(1)]
        public void GetTicketForDeletion_TicketIsReturnedAndIdsMatch(int id)
        {
            Assert.IsTrue(_ticketBusinessLogic.GetTicketForDeletion(id) is Ticket t && t.Id == id);
        }

        [TestMethod]
        [DataRow(0)]
        public void GetTicketForDeletion_ThrowsNullReferenceExceptionWhenTicketNull(int id)
        {
            Assert.ThrowsException<NullReferenceException>(() => _ticketBusinessLogic.GetTicketForDeletion(id));
        }

        [TestMethod]
        [DataRow(1)]
        public void DeleteTicket_DeleteOperationIsPerformedOnTicket(int id)
        {
            _ticketBusinessLogic.DeleteTicket(id);

            Assert.IsFalse(_ticketData.Any(t => t.Id == id));
        }

        [TestMethod]
        [DataRow(0)]
        public void DeleteTicket_NoDeleteOperationWhenTicketNull(int id)
        {
            int oldCount = _ticketData.Count;
            _ticketBusinessLogic.DeleteTicket(id);

            Assert.AreEqual(oldCount, _ticketData.Count);
        }

        [TestMethod]
        [DataRow(5, "Hopefully this passes")]
        public async Task CommentTicketAsync_CommentIsAddedToTicket(int id, string text)
        {
            await _ticketBusinessLogic.CommentTicketAsync(id, text, _principal);

            Assert.IsTrue(_commentData.Any(c => c.TicketId == id && c.CreatedById == _primaryUser.Id && c.Description == text));
        }

        [TestMethod]
        public async Task CommentTicketAsync_ThrowsNullReferenceExceptionWhenUserNull()
        {
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _ticketBusinessLogic.CommentTicketAsync(1, "Test", new ClaimsPrincipal()));
        }

        [TestMethod]
        [DataRow(1, 20)]
        [DataRow(4, 15)]
        public void UpdateHours_SetsTicketsHours(int id, int hours)
        {
            _ticketBusinessLogic.UpdateHours(id, hours);

            Ticket ticket = _ticketData.First(t => t.Id == id);

            Assert.AreEqual(ticket.RequiredHours, hours);
        }

        [TestMethod]
        [DataRow(0)]
        public void UpdateHours_ThrowsNullReferenceExceptionWhenTicketNull(int id)
        {
            Assert.ThrowsException<NullReferenceException>(() => _ticketBusinessLogic.UpdateHours(id, 25));
        }

        [TestMethod]
        [DataRow(1)]
        public async Task WatchTicketAsync_UserBecomesTicketWatcher(int id)
        {
            await _ticketBusinessLogic.WatchTicketAsync(id, _principal);

            Assert.IsTrue(_ticketWatcherData.Any(t => t.TicketId == id && t.WatcherId == _primaryUser.Id));
        }

        [TestMethod]
        public async Task WatchTicketAsync_ThrowsNullReferenceExceptionWhenUserNull()
        {
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () => await _ticketBusinessLogic.WatchTicketAsync(_ticketData[0].Id, new ClaimsPrincipal()));
        }

        [TestMethod]
        [DataRow(3)]
        public async Task UnwatchTicketAsync_UserStopsWatchingTicket(int id)
        {
            await _ticketBusinessLogic.UnwatchTicketAsync(id, _principal);

            Assert.IsFalse(_ticketWatcherData.Any(t => t.TicketId == id && t.WatcherId == _primaryUser.Id));
        }

        [TestMethod]
        public async Task UnwatchTicketAsync_ThrowsArgumentNullExceptionWhenIdNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                async () => await _ticketBusinessLogic.UnwatchTicketAsync(null, _principal));
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(5, false)]
        public void UpdateTicketCompleted_SetsTicketCompletedProp(int id, bool isCompleted)
        {
            _ticketBusinessLogic.UpdateTicketCompleted(id, isCompleted);

            Ticket ticket = _ticketData.First(t => t.Id == id);

            Assert.AreEqual(ticket.Completed, isCompleted);
        }

        [TestMethod]
        public void UpdateTicketCompleted_ThrowsArgumentNullExceptionWhenIdNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _ticketBusinessLogic.UpdateTicketCompleted(null, true));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void TicketExists_ExistingTicketReturnsTrue(int id)
        {
            Assert.IsTrue(_ticketBusinessLogic.TicketExists(id));
        }

        [TestMethod]
        [DataRow(0)]
        public void TicketExists_NullTicketReturnsFalse(int id)
        {
            Assert.IsFalse(_ticketBusinessLogic.TicketExists(id));
        }
    }
}