﻿using SocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class RecordsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string id)
        {
            if (id == null) // если специальное имя пользователя не указано
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users viewing_user = users.getUserFromUserSpecialName(id); // пользователь, страница которого открыта

            if (viewing_user == null) // если указанного пользователя не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            ViewBag.ViewingUser = viewing_user;

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;

            Dictionary<PermissionsToUser, bool> userPermissionsToUser = users.getUserPermissionsToUser(user, viewing_user);

            if (!userPermissionsToUser[PermissionsToUser.CAN_CREATE_RECORDS_ON_MY_PAGE])
            {
                return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
            }

            if (Request.Form["ok"] != null) // если была нажата кнопка добавления записи
            {
                try
                {
                    ViewBag.text = Request.Form["text"];

                    objects object_record = new objects();
                    object_record.object_type_id = Convert.ToInt32(ObjectsTypes.RECORD);
                    object_record.user_id_from = user.id;
                    object_record.creation_datetime_int = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                    MyFunctions.database.objects.Add(object_record);
                    MyFunctions.database.SaveChanges(); // сохраняем изменения, чтобы установился id для объекта

                    records record = new records();
                    record.object_id = object_record.id;
                    record.user_id_to = viewing_user.id;
                    record.text = ViewBag.text;
                    record.attached_record_id = -1;
                    MyFunctions.database.records.Add(record);
                    MyFunctions.database.SaveChanges();

                    return RedirectToAction("Viewing", "Users", new { id = id }); // перенаправляем пользователя
                }
                catch
                {
                    List<string> errors = new List<string>();
                    errors.Add("Недопустимый текст! Можно вводить только обычный текст, использование HTML-тегов не разрешено!");
                    ViewBag.Errors = errors;
                }
            }

            return View();
        }

        public ActionResult Edit(string id)
        {
            int record_id = Convert.ToInt32(id);
            if (MyFunctions.database.records.Where(p => (p.id == record_id)).Count() == 0) // если указанной записи не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)
            
            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_EDIT] == false)
            {
                return RedirectToAction("Viewing", "Records", new { id = id }); // перенаправляем пользователя
            }
            
            if (Request.Form["ok"] != null) // если была нажата кнопка изменения записи
            {
                try
                {
                    ViewBag.text = Request.Form["text"];

                    record.text = ViewBag.text;
                    MyFunctions.database.SaveChanges();

                    return RedirectToAction("Viewing", "Records", new { id = id }); // перенаправляем пользователя
                }
                catch
                {
                    List<string> errors = new List<string>();
                    errors.Add("Недопустимый текст! Можно вводить только обычный текст, использование HTML-тегов не разрешено!");
                    ViewBag.Errors = errors;
                }
            }

            ViewBag.text = record.text;

            ViewBag.User = user;
            ViewBag.ViewingUser = MyFunctions.database.users.Where(p => (p.id == record.user_id_to)).FirstOrDefault();
            ViewBag.Record = record;

            return View();
        }

        public ActionResult Viewing(string id, string rating_action, string commentary_action = null, int commentary_id = -1)
        {
            int record_id = Convert.ToInt32(id);
            if (MyFunctions.database.records.Where(p => (p.id == record_id)).Count() == 0) // если указанной записи не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            ViewBag.User = user;
                        
            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            users viewing_user = MyFunctions.database.users.Where(p => (p.id == record.user_id_to)).FirstOrDefault();

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_SEE] == false)
            {
                return RedirectToAction("Viewing", "Users", new { id = viewing_user.special_name }); // перенаправляем пользователя
            }

            if (rating_action == "up_rating")
            {
                MyFunctions.changeObjectRating(record, user, false);
            }
            else if (rating_action == "up_rating_plus_return_to_user_page")
            {
                MyFunctions.changeObjectRating(record, user, false);
                return RedirectToAction("Viewing", "Users", new { id = viewing_user.special_name }); // перенаправляем пользователя
            }
            else if (rating_action == "down_rating")
            {
                MyFunctions.changeObjectRating(record, user, true);
            }
            else if (rating_action == "down_rating_plus_return_to_user_page")
            {
                MyFunctions.changeObjectRating(record, user, true);
                return RedirectToAction("Viewing", "Users", new { id = viewing_user.special_name }); // перенаправляем пользователя
            }

            if (commentary_action != null)
            {
                commentaries commentary = MyFunctions.database.commentaries.Where(p => (p.id == commentary_id)).FirstOrDefault();
                if (commentary_action == "delete")
                {
                    MyFunctions.database.commentaries.Remove(commentary);
                    MyFunctions.database.SaveChanges();
                }
                else if (commentary_action == "up_rating")
                {
                    MyFunctions.changeObjectRating(commentary, user, false);
                }
                else if (commentary_action == "down_rating")
                {
                    MyFunctions.changeObjectRating(commentary, user, true);
                }
            }

            if (Request.Form["ok"] != null) // если была нажата кнопка изменения записи
            {
                try
                {
                    ViewBag.text = Request.Form["text"];

                    objects object_commentary = new objects();
                    object_commentary.object_type_id = Convert.ToInt32(ObjectsTypes.COMMENTARY);
                    object_commentary.user_id_from = user.id;
                    object_commentary.creation_datetime_int = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
                    MyFunctions.database.objects.Add(object_commentary);
                    MyFunctions.database.SaveChanges(); // сохраняем изменения, чтобы установился id для объекта

                    commentaries new_commentary = new commentaries();
                    new_commentary.object_id = object_commentary.id;
                    new_commentary.text = ViewBag.text;
                    MyFunctions.database.commentaries.Add(new_commentary);
                    MyFunctions.database.SaveChanges();

                    commentaries_to_objects_with_commentaries commentary_info = new commentaries_to_objects_with_commentaries();
                    commentary_info.commentary_id = new_commentary.id;
                    commentary_info.object_id = record.object_id;
                    MyFunctions.database.commentaries_to_objects_with_commentaries.Add(commentary_info);
                    MyFunctions.database.SaveChanges();

                    return RedirectToAction("Viewing", "Records", new { id = id }); // перенаправляем пользователя
                }
                catch
                {
                    List<string> errors = new List<string>();
                    errors.Add("Недопустимый текст! Можно вводить только обычный текст, использование HTML-тегов не разрешено!");
                    ViewBag.Errors = errors;
                }
            }

            List<commentaries> list = MyFunctions.getCommentaries(record);
            List<object> list_object = list.ToList<object>();
            ViewBag.ListOnPage = list_object;

            ViewBag.User = user;
            ViewBag.ViewingUser = viewing_user;
            ViewBag.Record = record;

            return View();
        }

        public ActionResult Delete(string id)
        {
            int record_id = Convert.ToInt32(id);
            if (MyFunctions.database.records.Where(p => (p.id == record_id)).Count() == 0) // если указанной записи не существует
            {
                return RedirectToAction("Index", "Users"); // перенаправляем пользователя
            }

            users user = users.getUserFromUserId(Convert.ToInt32(Session["id"])); // пользователь, просматривающий страницу (может совпадать с пользователем, страница которого открыта)

            records record = MyFunctions.database.records.Where(p => (p.id == record_id)).FirstOrDefault();
            Dictionary<SocialNetwork.Models.PermissionsToObject, bool> userPermissionsToRecord = SocialNetwork.Models.users.getUserPermissionsToObject(user, record);

            if (userPermissionsToRecord[SocialNetwork.Models.PermissionsToObject.CAN_DELETE] == false)
            {
                return RedirectToAction("Viewing", "Records", new { id = id }); // перенаправляем пользователя
            }

            users viewing_user = MyFunctions.database.users.Where(p => (p.id == record.user_id_to)).FirstOrDefault();

            if (Request.Form["ok"] != null) // если была нажата кнопка удаления записи
            {
                ViewBag.text = Request.Form["text"];

                MyFunctions.database.records.Remove(record);
                MyFunctions.database.SaveChanges();

                return RedirectToAction("Viewing", "Users", new { id = viewing_user.special_name }); // перенаправляем пользователя
            }

            ViewBag.User = user;
            ViewBag.ViewingUser = viewing_user;
            ViewBag.Record = record;

            return View();
        }
    }
}