using System;
using System.Collections.Generic;
using System.Threading;
using Auth.Core;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Rhino.Mocks;

namespace Auth.Tests
{
	[TestFixture]
	public class TokenControllerTests
	{
		[Test]
		public void TokenTest()
		{
			var options = MockRepository.GenerateStub<IOptions<AuthOptions>>();
			options.Expect(o => o.Value)
				.Return(new AuthOptions { TokenTimeToLife = TimeSpan.FromMilliseconds(100) }); // Время жизни токена - 100 мс

			var tokenController =
				new TokenController(options); 

			var userGuid = 123456;

			var firstToken = tokenController.CreateOrUpdateToken(userGuid);
			var secondToken = tokenController.CreateOrUpdateToken(userGuid);
			// При повторном запросе токена для одного пользователя, возвращается один и тот же, пока он актуален
			Assert.AreEqual(firstToken, secondToken);

			var token = tokenController.CreateOrUpdateToken(userGuid);
			Thread.Sleep(200); // Первый токен протух
			var anotherToken = tokenController.CreateOrUpdateToken(userGuid);
			// Теперь они разные
			Assert.AreNotEqual(token, anotherToken);

			var error = new KeyValuePair<string, string>();
			try
			{
				tokenController.GetUserId(token);
			}
			catch (AuthException ex)
			{
				error = ex.ErrorCodeWithDescription;
			}
			Assert.AreEqual(error, Errors.TokenIsNonActual);

			try
			{
				tokenController.GetUserId(Guid.Empty);
			}
			catch (AuthException ex)
			{
				error = ex.ErrorCodeWithDescription;
			}
			Assert.AreEqual(error, Errors.TokenIsEmpty);

			// Если периодически обращаться к токену, он не будет протухать
			userGuid = 654321;
			token = tokenController.CreateOrUpdateToken(userGuid);
			Assert.AreEqual(token, tokenController.GetTokenForUser(userGuid));
			Thread.Sleep(50);
			Assert.AreEqual(token, tokenController.GetTokenForUser(userGuid));
			Thread.Sleep(50);
			Assert.AreEqual(token, tokenController.GetTokenForUser(userGuid));
			Thread.Sleep(50);
			Assert.AreEqual(token, tokenController.GetTokenForUser(userGuid));
			Assert.AreEqual(userGuid, tokenController.GetUserId(token));

			Thread.Sleep(150);
			Assert.IsNull(tokenController.GetTokenForUser(userGuid));
		}

		[Test]
		public void RemoveTokenTest()
		{
			var options = MockRepository.GenerateStub<IOptions<AuthOptions>>();
			options.Expect(o => o.Value)
				.Return(new AuthOptions { TokenTimeToLife = TimeSpan.FromHours(1) }); // Время жизни токена - 1 час
			
			var tokenController = new TokenController(options);

			// пробуем удалить несуществующий токен
			tokenController.RemoveToken(Guid.NewGuid());

			var userId = 123456;
			var tokenId = tokenController.CreateOrUpdateToken(userId);              // создаём токен
			Assert.AreEqual(tokenId, tokenController.GetTokenForUser(userId));      // убеждаемся что токен создался
			tokenController.RemoveToken(tokenId);                                   // удаляем токен
			Assert.IsNull(tokenController.GetTokenForUser(userId));                 // убеждаемся что токена больше нет
		}
	}
}