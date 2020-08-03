using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using DevOpsDeploy.Data;
using DevOpsDeploy.Models;
using DevOpsDeploy.Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DevOpsDeploy.Test.Controllers {
    public class ProjectResourceIntTest {
        public ProjectResourceIntTest()
        {
            _factory = new NhipsterWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _applicationDatabaseContext = _factory.GetRequiredService<ApplicationDatabaseContext>();

            InitTest();
        }

        private const string DefaultName = "AAAAAAAAAA";
        private const string UpdatedName = "BBBBBBBBBB";

        private readonly NhipsterWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private Project _project;

        private Project CreateEntity()
        {
            return new Project {
                Name = DefaultName
            };
        }

        private void InitTest()
        {
            _project = CreateEntity();
        }

        [Fact]
        public async Task CreateProject()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Projects.Count();

            // Create the Project
            var response = await _client.PostAsync("/api/projects", TestUtil.ToJsonContent(_project));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Project in the database
            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testProject = projectList[projectList.Count - 1];
            testProject.Name.Should().Be(DefaultName);
        }

        [Fact]
        public async Task CreateProjectWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Projects.Count();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Project with an existing ID
            _project.Id = "1";

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/projects", TestUtil.ToJsonContent(_project));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Project in the database
            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckNameIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.Projects.Count();

            // Set the field to null
            _project.Name = null;

            // Create the Project, which fails.
            var response = await _client.PostAsync("/api/projects", TestUtil.ToJsonContent(_project));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllProjects()
        {
            // Initialize the database
            _applicationDatabaseContext.Projects.Add(_project);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the projectList
            var response = await _client.GetAsync("/api/projects?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_project.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetProject()
        {
            // Initialize the database
            _applicationDatabaseContext.Projects.Add(_project);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the project
            var response = await _client.GetAsync($"/api/projects/{_project.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_project.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetNonExistingProject()
        {
            var response = await _client.GetAsync("/api/projects/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateProject()
        {
            // Initialize the database
            _applicationDatabaseContext.Projects.Add(_project);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Projects.Count();

            // Update the project
            var updatedProject =
                await _applicationDatabaseContext.Projects.SingleOrDefaultAsync(it => it.Id == _project.Id);
            // Disconnect from session so that the updates on updatedProject are not directly saved in db
//TODO detach
            updatedProject.Name = UpdatedName;

            var response = await _client.PutAsync("/api/projects", TestUtil.ToJsonContent(updatedProject));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Project in the database
            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testProject = projectList[projectList.Count - 1];
            testProject.Name.Should().Be(UpdatedName);
        }

        [Fact]
        public async Task UpdateNonExistingProject()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Projects.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/projects", TestUtil.ToJsonContent(_project));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Project in the database
            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteProject()
        {
            // Initialize the database
            _applicationDatabaseContext.Projects.Add(_project);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Projects.Count();

            var response = await _client.DeleteAsync($"/api/projects/{_project.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var projectList = _applicationDatabaseContext.Projects.ToList();
            projectList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Project));
            var project1 = new Project {
                Id = "1"
            };
            var project2 = new Project {
                Id = project1.Id
            };
            project1.Should().Be(project2);
            project2.Id = "2";
            project1.Should().NotBe(project2);
            project1.Id = "0";
            project1.Should().NotBe(project2);
        }
    }
}
