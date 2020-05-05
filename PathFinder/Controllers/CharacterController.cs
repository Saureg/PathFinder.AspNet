﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PathFinder.Data.Interfaces;
using PathFinder.Data.Models;
using PathFinder.ViewModels;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace PathFinder.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ICharacter _character;
        private readonly IAllRaces _allRaces;
        private readonly IAllClasses _allClasses;

        public CharacterController(ICharacter character, IAllRaces allRaces, IAllClasses allClasses)
        {
            _character = character;
            _allRaces = allRaces;
            _allClasses = allClasses;
        }

        public IActionResult Create()
        {
            var races = _allRaces.Races;
            
            var characterViewModel = new CharacterCreateViewModel
            {
                RaceSelectList = new SelectList(races, "Id", "Name"),
                CharClassSelectList = new SelectList(_allClasses.CharClasses, "Id", "Name")
            };
            
            ViewData["Title"] = "Новый персонаж";
            
            return View(characterViewModel);
        }
        
        [HttpPost]
        public IActionResult Create(Character character)
        {
            if (ModelState.IsValid)
            {
                _character.CreateCharacter(character);
                return RedirectToAction("Complete");
            }

            var characterViewModel = new CharacterCreateViewModel
            {
                Character = character,
                RaceSelectList = new SelectList(_allRaces.Races, "Id", "Name"),
                CharClassSelectList = new SelectList(_allClasses.CharClasses, "Id", "Name")
            };

            return View(characterViewModel);
        }
        
        public IActionResult Complete()
        {
            ViewBag.Message = "Персонаж успешно создан!";
            return View();
        }

        [Route("Character/List")]
        public ViewResult List()
        {
            IEnumerable<Character> characters = _character.Characters.OrderBy(c => c.Id);
            
            var characterViewModel = new CharacterListViewModel
            {
                Characters = characters,
                Races = _allRaces.Races.ToList(),
                CharClasses = _allClasses.CharClasses.ToList()
            };
            
            ViewData["Title"] = "Персонажи";
            
            return View(characterViewModel);
        }
    }
}