using System;
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
    public class DeploymentResourceIntTest {
        public DeploymentResourceIntTest()
        {
            _factory = new NhipsterWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _applicationDatabaseContext = _factory.GetRequiredService<ApplicationDatabaseContext>();

            InitTest();
        }

        private static readonly DateTime DefaultDeployedAt = DateTime.UnixEpoch;
        private static readonly DateTime UpdatedDeployedAt = DateTime.Now;

        private readonly NhipsterWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private Deployment _deployment;

        private Deployment CreateEntity()
        {
            return new Deployment {
                DeployedAt = DefaultDeployedAt
            };
        }

        private void InitTest()
        {
            _deployment = CreateEntity();
        }

        [Fact]
        public async Task CreateDeployment()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Deployments.Count();

            // Create the Deployment
            var response = await _client.PostAsync("/api/deployments", TestUtil.ToJsonContent(_deployment));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Deployment in the database
            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testDeployment = deploymentList[deploymentList.Count - 1];
            testDeployment.DeployedAt.Should().Be(DefaultDeployedAt);
        }

        [Fact]
        public async Task CreateDeploymentWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Deployments.Count();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Deployment with an existing ID
            _deployment.Id = "1";

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/deployments", TestUtil.ToJsonContent(_deployment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Deployment in the database
            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckDeployedAtIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.Deployments.Count();

            // Set the field to null
            _deployment.DeployedAt = DateTime.Now;

            // Create the Deployment, which fails.
            var response = await _client.PostAsync("/api/deployments", TestUtil.ToJsonContent(_deployment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllDeployments()
        {
            // Initialize the database
            _applicationDatabaseContext.Deployments.Add(_deployment);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the deploymentList
            var response = await _client.GetAsync("/api/deployments?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_deployment.Id);
            json.SelectTokens("$.[*].deployedAt").Should().Contain(DefaultDeployedAt);
        }

        [Fact]
        public async Task GetDeployment()
        {
            // Initialize the database
            _applicationDatabaseContext.Deployments.Add(_deployment);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the deployment
            var response = await _client.GetAsync($"/api/deployments/{_deployment.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_deployment.Id);
            json.SelectTokens("$.deployedAt").Should().Contain(DefaultDeployedAt);
        }

        [Fact]
        public async Task GetNonExistingDeployment()
        {
            var response = await _client.GetAsync("/api/deployments/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDeployment()
        {
            // Initialize the database
            _applicationDatabaseContext.Deployments.Add(_deployment);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Deployments.Count();

            // Update the deployment
            var updatedDeployment =
                await _applicationDatabaseContext.Deployments.SingleOrDefaultAsync(it => it.Id == _deployment.Id);
            // Disconnect from session so that the updates on updatedDeployment are not directly saved in db
//TODO detach
            updatedDeployment.DeployedAt = UpdatedDeployedAt;

            var response = await _client.PutAsync("/api/deployments", TestUtil.ToJsonContent(updatedDeployment));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Deployment in the database
            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testDeployment = deploymentList[deploymentList.Count - 1];
            testDeployment.DeployedAt.Should().Be(UpdatedDeployedAt);
        }

        [Fact]
        public async Task UpdateNonExistingDeployment()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Deployments.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/deployments", TestUtil.ToJsonContent(_deployment));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Deployment in the database
            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteDeployment()
        {
            // Initialize the database
            _applicationDatabaseContext.Deployments.Add(_deployment);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Deployments.Count();

            var response = await _client.DeleteAsync($"/api/deployments/{_deployment.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var deploymentList = _applicationDatabaseContext.Deployments.ToList();
            deploymentList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Deployment));
            var deployment1 = new Deployment {
                Id = "1"
            };
            var deployment2 = new Deployment {
                Id = deployment1.Id
            };
            deployment1.Should().Be(deployment2);
            deployment2.Id = "2";
            deployment1.Should().NotBe(deployment2);
            deployment1.Id = "0";
            deployment1.Should().NotBe(deployment2);
        }
    }
}
