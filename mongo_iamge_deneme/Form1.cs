using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;


namespace mongo_iamge_deneme
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb+srv://KaanKalkan:sabibdg2@newmongoproject.gshwhde.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("MytestDB");
            var fs = new GridFSBucket(database);

            var id = UploadFile(fs);

            DownloadFile(fs, id);


           // Console.WriteLine("The list of databases are:");

            /*foreach (var item in dbList)
            {
                listBox1.Items.Add(item);
            }*/
        }
        private static ObjectId UploadFile(GridFSBucket fs)
        {
            using (var s = File.OpenRead(@"C:\Users\Kaan\Desktop\image.jpeg"))
            {
                var t = Task.Run<ObjectId>(() => {
                    return
                    fs.UploadFromStreamAsync("image.jpeg", s);
                });

                return t.Result;
            }
        }

        private static void DownloadFile(GridFSBucket fs, ObjectId id)
        {
            //This works
            var t = fs.DownloadAsBytesByNameAsync("image.jpeg");
            Task.WaitAll(t);
            var bytes = t.Result;


            //This blows chunks (I think it's a driver bug, I'm using 2.1 RC-0)
            var x = fs.DownloadAsBytesAsync(id);
            Task.WaitAll(x);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb+srv://KaanKalkan:sabibdg2@newmongoproject.gshwhde.mongodb.net/?retryWrites=true&w=majority");
            var db = client.GetDatabase("MytestDB");
            var fs = new GridFSBucket(db);

            var fsfiles = db.GetCollection<BsonDocument>("fs.files");
            var filter = Builders<BsonDocument>.Filter.Eq("length", 9653);
            var doc = fsfiles.Find(filter).FirstOrDefault();
            listBox1.Items.Add(doc.ToString());

            using (var stream = doc.OpenRead())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                using (var newFs = new FileStream(newFileName, FileMode.Create))
                {
                    newFs.Write(bytes, 0, bytes.Length);
                    Image image = Image.FromStream(newFs);

                    f2.SizeMode = PictureBoxSizeMode.StretchImage;

                    f2.Image = image;

                }
            }

        }
    }
}
//63c7b2dc772f08450627f63b