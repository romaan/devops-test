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
    public class EnvironmentResourceIntTest {
        public EnvironmentResourceIntTest()
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

        private Environment _environment;

        private Environment CreateEntity()
        {
            return new Environment {
                Name = DefaultName
            };
        }

        private void InitTest()
        {
            _environment = CreateEntity();
        }

        [Fact]
        public async Task CreateEnvironment()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Environments.Count();

            // Create the Environment
            var response = await _client.PostAsync("/api/environments", TestUtil.ToJsonContent(_environment));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Environment in the database
            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testEnvironment = environmentList[environmentList.Count - 1];
            testEnvironment.Name.Should().Be(DefaultName);
        }

        [Fact]
        public async Task CreateEnvironmentWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Environments.Count();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Environment with an existing ID
            _environment.Id = "1";

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/environments", TestUtil.ToJsonContent(_environment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Environment in the database
            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckNameIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.Environments.Count();

            // Set the field to null
            _environment.Name = null;

            // Create the Environment, which fails.
            var response = await _client.PostAsync("/api/environments", TestUtil.ToJsonContent(_environment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllEnvironments()
        {
            // Initialize the database
            _applicationDatabaseContext.Environments.Add(_environment);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the environmentList
            var response = await _client.GetAsync("/api/environments?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_environment.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetEnvironment()
        {
            // Initialize the database
            _applicationDatabaseContext.Environments.Add(_environment);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the environment
            var response = await _client.GetAsync($"/api/environments/{_environment.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_environment.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetNonExistingEnvironment()
        {
            var response = await _client.GetAsync("/api/environments/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateEnvironment()
        {
            // Initialize the database
            _applicationDatabaseContext.Environments.Add(_environment);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Environments.Count();

            // Update the environment
            var updatedEnvironment =
                await _applicationDatabaseContext.Environments.SingleOrDefaultAsync(it => it.Id == _environment.Id);
            // Disconnect from session so that the updates on updatedEnvironment are not directly saved in db
//TODO detach
            updatedEnvironment.Name = UpdatedName;

            var response = await _client.PutAsync("/api/environments", TestUtil.ToJsonContent(updatedEnvironment));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Environment in the database
            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testEnvironment = environmentList[environmentList.Count - 1];
            testEnvironment.Name.Should().Be(UpdatedName);
        }

        [Fact]
        public async Task UpdateNonExistingEnvironment()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Environments.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/environments", TestUtil.ToJsonContent(_environment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Environment in the database
            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteEnvironment()
        {
            // Initialize the database
            _applicationDatabaseContext.Environments.Add(_environment);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Environments.Count();

            var response = await _client.DeleteAsync($"/api/environments/{_environment.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var environmentList = _applicationDatabaseContext.Environments.ToList();
            environmentList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Environment));
            var environment1 = new Environment {
                Id = "1"
            };
            var environment2 = new Environment {
                Id = environment1.Id
            };
            environment1.Should().Be(environment2);
            environment2.Id = "2";
            environment1.Should().NotBe(environment2);
            environment1.Id = "0";
            environment1.Should().NotBe(environment2);
        }
    }
}
