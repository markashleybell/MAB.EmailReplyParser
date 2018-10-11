using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace MAB.EmailReplyParser.Test
{
    [TestFixture]
    public class Tests
    {
        private string LoadFile(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        [TestCase("basic_test")]
        [TestCase("correct_sig")]
        [TestCase("email_1_1")]
        [TestCase("email_1_2")]
        [TestCase("email_1_3")]
        [TestCase("email_1_4")]
        [TestCase("email_1_5")]
        [TestCase("email_1_6")]
        [TestCase("email_1_7")]
        [TestCase("email_1_8")]
        [TestCase("email_1_9")]
        [TestCase("email_2_1")]
        [TestCase("email_2_2")]
        [TestCase("email_2_3")]
        [TestCase("email_BlackBerry")]
        [TestCase("email_bullets")]
        [TestCase("email_gmail")]
        [TestCase("email_headers_no_delimiter")]
        [TestCase("email_iPhone")]
        [TestCase("email_multi_word_sent_from_my_mobile_device")]
        [TestCase("email_one_is_not_on")]
        [TestCase("email_partial_quote_header")]
        [TestCase("email_sent_from_my_not_signature")]
        [TestCase("email_sig_delimiter_in_middle_of_line")]
        [TestCase("greedy_on")]
        [TestCase("pathological")]
        public void VerifyParsedReply(string fileName)
        {
            string rawBody = LoadFile(string.Format("MAB.EmailReplyParser.Test.TestData.{0}.txt", fileName));
            string expectedReply = LoadFile(string.Format("MAB.EmailReplyParser.Test.ExpectedResults.{0}.txt", fileName)).Replace("\r\n", "\n");

            string reply = EmailReplyParser.getReply(rawBody);

            Assert.AreEqual(expectedReply, reply);
        }
    }
}
