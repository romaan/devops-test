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
    public class ReleaseResourceIntTest {
        public ReleaseResourceIntTest()
        {
            _factory = new NhipsterWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _applicationDatabaseContext = _factory.GetRequiredService<ApplicationDatabaseContext>();

            InitTest();
        }

        private const string DefaultVersion = "AAAAAAAAAA";
        private const string UpdatedVersion = "BBBBBBBBBB";

        private static readonly DateTime DefaultCreated = DateTime.UnixEpoch;
        private static readonly DateTime UpdatedCreated = DateTime.Now;

        private readonly NhipsterWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private Release _release;

        private Release CreateEntity()
        {
            return new Release {
                Version = DefaultVersion,
                Created = DefaultCreated
            };
        }

        private void InitTest()
        {
            _release = CreateEntity();
        }

        [Fact]
        public async Task CreateRelease()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Releases.Count();

            // Create the Release
            var response = await _client.PostAsync("/api/releases", TestUtil.ToJsonContent(_release));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Release in the database
            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testRelease = releaseList[releaseList.Count - 1];
            testRelease.Version.Should().Be(DefaultVersion);
            testRelease.Created.Should().Be(DefaultCreated);
        }

        [Fact]
        public async Task CreateReleaseWithExistingId()
        {
            var databaseSizeBeforeCreate = _applicationDatabaseContext.Releases.Count();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Release with an existing ID
            _release.Id = "1";

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/releases", TestUtil.ToJsonContent(_release));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Release in the database
            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckVersionIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.Releases.Count();

            // Set the field to null
            _release.Version = null;

            // Create the Release, which fails.
            var response = await _client.PostAsync("/api/releases", TestUtil.ToJsonContent(_release));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task CheckCreatedIsRequired()
        {
            var databaseSizeBeforeTest = _applicationDatabaseContext.Releases.Count();

            // Set the field to null
            _release.Created = DateTime.Now;

            // Create the Release, which fails.
            var response = await _client.PostAsync("/api/releases", TestUtil.ToJsonContent(_release));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllReleases()
        {
            // Initialize the database
            _applicationDatabaseContext.Releases.Add(_release);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get all the releaseList
            var response = await _client.GetAsync("/api/releases?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_release.Id);
            json.SelectTokens("$.[*].version").Should().Contain(DefaultVersion);
            json.SelectTokens("$.[*].created").Should().Contain(DefaultCreated);
        }

        [Fact]
        public async Task GetRelease()
        {
            // Initialize the database
            _applicationDatabaseContext.Releases.Add(_release);
            await _applicationDatabaseContext.SaveChangesAsync();

            // Get the release
            var response = await _client.GetAsync($"/api/releases/{_release.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_release.Id);
            json.SelectTokens("$.version").Should().Contain(DefaultVersion);
            json.SelectTokens("$.created").Should().Contain(DefaultCreated);
        }

        [Fact]
        public async Task GetNonExistingRelease()
        {
            var response = await _client.GetAsync("/api/releases/" + long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateRelease()
        {
            // Initialize the database
            _applicationDatabaseContext.Releases.Add(_release);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Releases.Count();

            // Update the release
            var updatedRelease =
                await _applicationDatabaseContext.Releases.SingleOrDefaultAsync(it => it.Id == _release.Id);
            // Disconnect from session so that the updates on updatedRelease are not directly saved in db
            //TODO detach
            updatedRelease.Version = UpdatedVersion;
            updatedRelease.Created = UpdatedCreated;

            var response = await _client.PutAsync("/api/releases", TestUtil.ToJsonContent(updatedRelease));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Release in the database
            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testRelease = releaseList[releaseList.Count - 1];
            testRelease.Version.Should().Be(UpdatedVersion);
            testRelease.Created.Should().Be(UpdatedCreated);
        }

        [Fact]
        public async Task UpdateNonExistingRelease()
        {
            var databaseSizeBeforeUpdate = _applicationDatabaseContext.Releases.Count();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/releases", TestUtil.ToJsonContent(_release));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Release in the database
            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteRelease()
        {
            // Initialize the database
            _applicationDatabaseContext.Releases.Add(_release);
            await _applicationDatabaseContext.SaveChangesAsync();

            var databaseSizeBeforeDelete = _applicationDatabaseContext.Releases.Count();

            var response = await _client.DeleteAsync($"/api/releases/{_release.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var releaseList = _applicationDatabaseContext.Releases.ToList();
            releaseList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Release));
            var release1 = new Release {
                Id = "1"
            };
            var release2 = new Release {
                Id = release1.Id
            };
            release1.Should().Be(release2);
            release1.Id = "2";
            release1.Should().NotBe(release2);
            release1.Id = "0";
            release1.Should().NotBe(release2);
        }
    }
}
