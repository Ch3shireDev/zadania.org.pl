using Microsoft.AspNetCore.Mvc;

namespace ProblemLibrary
{
    public class TestUser
    {
        public TestUser()
        {
        }

        public TestUser(string profile)
        {
        }

        [BindProperty(Name = "given_name", SupportsGet = true)]
        private string Name { get; set; }

        //    "given_name": "Igor",
        //    "family_name": "Nowicki",
        //    "nickname": "thesmilingcatofcheshire",
        //    "name": "Igor Nowicki",
        //    "picture": "https://lh3.googleusercontent.com/a-/AOh14GiVojheEsk775EqqqBEdjp37xbrAZOGWOrO2T9cDg",
        //    "locale": "pl",
        //    "updated_at": "2020-07-31T21:44:59.531Z",
        //    "email": "thesmilingcatofcheshire@gmail.com",
        //    "email_verified": true,
        //    "sub": "google-oauth2|107032790204451886763"
    }
}