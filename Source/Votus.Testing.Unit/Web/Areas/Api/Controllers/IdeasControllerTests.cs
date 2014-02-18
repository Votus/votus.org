using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Controllers;
using Votus.Web.Areas.Api.Models;
using Xunit;

namespace Votus.Testing.Unit.Web.Areas.Api.Controllers
{
    public class IdeasControllerTests
    {
        private readonly IdeasController        _ideasController;
        private readonly QueueManager           _fakeCommandDispatcher;
        private readonly IPartitionedRepository _fakeIdeasRepo;

        public IdeasControllerTests()
        {
            _fakeCommandDispatcher  = A.Fake<QueueManager>();
            _fakeIdeasRepo = A.Fake<IPartitionedRepository>();

            _ideasController = new IdeasController {
                CommandDispatcher = _fakeCommandDispatcher,
                IdeasRepository   = _fakeIdeasRepo
            };
        }

        [Fact]
        public
        async Task
        GetIdeasAsync_ExcludeTagNotSpecified_ReturnsAllIdeas()
        {
            // Arrange
            const string excludedTag = "votus-test";

            var allIdeas = new List<IdeaViewModel> {
                new IdeaViewModel { Tag = ""  },
                new IdeaViewModel { Tag = "cool-tag"  },
                new IdeaViewModel { Tag = excludedTag }
            };

            var pagedResult = new PagedResult<IdeaViewModel> {
                Page = allIdeas
            };

            A.CallTo(() =>
                _fakeIdeasRepo.GetAllPagedAsync<IdeaViewModel>(A<string>.Ignored, A<int>.Ignored)
            ).ReturnsCompletedTask(pagedResult);

            // Act
            var ideas = await _ideasController.GetIdeasAsync();

            // Assert
            Assert.Equal(3, ideas.Page.Count());
        }
        
        [Fact]
        public 
        async Task 
        GetIdeasAsync_ResultsSpanTwoRepoPages_ReturnsIdeasAsOnePage()
        {
            // Arrange
            const string excludedTag = "votus-test";

            var pagedResult1 = new PagedResult<IdeaViewModel> {
                Page = new List<IdeaViewModel> {
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = ""           },
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = excludedTag  }
                },
                NextPageToken = "Page2"
            };

            var pagedResult2 = new PagedResult<IdeaViewModel> {
                Page = new List<IdeaViewModel> {
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = excludedTag  },
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = "cool-tag" }, 
                }
            };

            A.CallTo(() => _fakeIdeasRepo.GetAllPagedAsync<IdeaViewModel>(null,                       A<int>.Ignored)).ReturnsCompletedTask(pagedResult1);
            A.CallTo(() => _fakeIdeasRepo.GetAllPagedAsync<IdeaViewModel>(pagedResult1.NextPageToken, A<int>.Ignored)).ReturnsCompletedTask(pagedResult2);

            // Act
            var recentIdeas = await _ideasController.GetIdeasAsync(
                excludeTag: excludedTag, 
                itemsPerPage: 2
            );

            // Assert
            Assert.Equal(2, recentIdeas.Page.Count());
        }

        [Fact]
        public
        async Task
        GetIdeasAsync_RepoResultsContainMoreThanTheMaxItems_ReturnsOnlyMaxItems()
        {
            // Arrange
            var pagedResult1 = new PagedResult<IdeaViewModel>
            {
                Page = new List<IdeaViewModel> {
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = ""        },
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = "one-tag" },
                    new IdeaViewModel { Id = Guid.NewGuid(), Tag = "two-tag" }
                }
            };

            A.CallTo(() => _fakeIdeasRepo.GetAllPagedAsync<IdeaViewModel>(null, A<int>.Ignored)).ReturnsCompletedTask(pagedResult1);
            
            // Act
            var actual = await _ideasController.GetIdeasAsync(itemsPerPage: 2);

            //Assert
            Assert.Equal(2, actual.Page.Count);
        }

        [Fact]
        public 
        void 
        FilterResultsByTag_TagSpecified_MatchedIdeasAreRemoved()
        {
            // Arrange
            const string tag = "some-tag";

            var ideasPagedResult = new List<IdeaViewModel> {new IdeaViewModel {Tag = tag}};

            // Act
            ideasPagedResult = IdeasController.FilterIdeasByTag(
                ideasPagedResult, 
                tag
            );

            // Assert
            Assert.False(ideasPagedResult.Any(idea => idea.Tag == tag));
        }

        [Fact]
        public
        void
        FilterResultsByTag_TagSpecified_MismatchedIdeasAreNotRemoved()
        {
            // Arrange
            const string tag = "some-tag";

            var ideasPagedResult = new List<IdeaViewModel> {new IdeaViewModel {Tag = tag}};

            // Act
            IdeasController.FilterIdeasByTag(ideasPagedResult, "some-other-tag");

            // Assert
            Assert.True(ideasPagedResult.Any(idea => idea.Tag == tag));
        }

        [Fact]
        public
        void 
        AssembleNextPageControllerToken_SkipIs0_SkipIsOmittedFromToken()
        {
            // Arrange
            const int skip         = 0;
            const string repoToken = "token";
            const char delimiter   = ':';

            // Act
            var actual = IdeasController.AssembleNextPageControllerToken(
                repoToken,
                skip,
                delimiter
            );

            // Assert
            Assert.Equal(repoToken, actual);
        }
    }
}
