﻿using Microsoft.AspNetCore.Mvc;
using Natanael.Contracts.V1;
using Natanael.Contracts.V1.Requests;
using Natanael.Contracts.V1.Responses;
using Natanael.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            this._identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Erros = ModelState.Values.SelectMany(a => a.Errors.Select(b => b.ErrorMessage))
                });
            }


            var authResponse = await this._identityService.RegisterAsync(request.Email, request.Password);

            if(!authResponse.Sucess)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Erros = authResponse.Erros
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await this._identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Sucess)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Erros = authResponse.Erros
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await this._identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Sucess)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Erros = authResponse.Erros
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }


    }
}
