using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Golf Directions",
                 "Golf Directions",
                 "Assets/10.jpg",
                 "Golf is a precision club and ball sport in which competing players (or golfers) use many types of clubs to hit balls into a series of holes on a course using the fewest number of strokes. Golf is defined, in the rules of golf, as playing a ball with a club from the teeing ground into the hole by a stroke or successive strokes in accordance with the Rules.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Best Golf Courses",
                 "Best Golf Courses",
                 "Assets/11.jpg",
                 "Best Golf Courses: Bringing the best in every pro golfer What makes a great golf course? If a golf course is filled with rich history, state of the art facilities, masterfully crafted landscapes",
                 "Best Golf Courses: Bringing the best in every pro golfer What makes a great golf course? If a golf course is filled with rich history, state of the art facilities, masterfully crafted landscapes (that pushes the boundary of excellence among pro golfers), lush sceneries and, of course, thriving popularity, then it’s definitely among the best golf courses. While there are many ways to know which greens are the best among the rest, it’s hard for anyone to decide which one to put on their ‘Best List’. Each golf enthusiast has his or her favorite pick. Another factor that adds to the difficulty of choosing the best is the number of golf courses to choose from—with some just opening recently. Here are two of the best golf courses one can ever play the game of golf in. They are considered as such not only for the degree of difficulty they impose on golfers but also for those extra features that makes them an enjoyable golfing conquest\n\nThe tar heel state is not really famous for golf. But there is one true-blue golf club beyond the outskirts of a bustling city. Located on the eastern part of North Carolina, the Eagle Point Golf Club never ceases to amaze pro and amateur golfers alike. This Tom Fazio-designed par-72 golf club is replete with amenities like tennis court and swimming pool. The club prides itself with its easily accessible greens—you can choose to walk as you go through different holes. On top of it, they have a very fine caddie program to the delight of their guests.\n\nWith more than 7,000 yards of golfing grounds— with a 74.5 rating and replete with 137 slope —the Eagle Point is simple the place where golfers could test their mettle. Adding to the challenge is the prevailing wind from the Atlantic Ocean. The club may look traditional, but it features some state of the art facilities. For those who want to hone their golf skills, a high-tech learning center is available. Yet, Eagle Point maintains a low-profile identity. Despite the fact that they are underrated, they are one of the best golf destinations in the country, what with the town’s best year-round golf weather and great private retreat for golfers and their families. The Concession in Florida, on the other hand, is called as such because of the history between two people who have created it. Opened In 2006, The Concession is the masterpiece of legendary golfers Jack Nicklaus and Tony Jacklin. Since its opening the club has always been included in the best modern courses list of different publications like Golfweek. \n\nThe name was derived from what transpired in 1969 when Nicklaus yielded a three-foot putt. That act of sportsmanship was all the British team (led by Jacklin) needed to tie with the Americans. While the Americans eventually won in the tournament, the act was forever etched in the annals of the sporting world. Decades after, the golf legends have worked on a 1,200-acre property that is free from housing developments. The par-72 Concession (7,470 yards, with 155 slopes and 77.6 rating) may be hard to conquer for its landscape angles and fairway bunkers. But this makes it one of the best golf courses—it’s challenging, yet enjoyable. And if you need some tips, just ask pro Jimmy Wright at the practice range.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Buying your Golf Club Bag",
                 "Buying your Golf Club Bag",
                 "Assets/12.jpg",
                 "Buying your Golf Club Bag Golf is a sport, one of the most popular leisure activities. Simply put, golf is a game played using different golf clubs on customized 9 hole or 18 hole golf courses. A golf player starts from a teeing ground and with a stroke or multiple strokes puts the ball into the hole.",
                 "Buying your Golf Club Bag Golf is a sport, one of the most popular leisure activities. Simply put, golf is a game played using different golf clubs on customized 9 hole or 18 hole golf courses. A golf player starts from a teeing ground and with a stroke or multiple strokes puts the ball into the hole. In between the teeing ground and the hole are different obstacles and different terrain types. Because of the varying conditions of where a player would hit his or her ball, there are a number of different golf clubs he or she can use. These include the driver, putter and irons. These clubs usually have specialized purposes and offers the player a variety of options for a shot. Because there are a lot of clubs a player typically needs a golf club bag to hold all these clubs. Buying a golf club bag is essential in playing golf to carry all the necessary equipments the player would use to complete the whole course. It is used to transport all your clubs and other paraphernalia. Overall, a golf club bag would make your golfing experience much more enjoyable and comfortable even though it does not necessarily affect your score. This is why it is important to purchase a bag that suits you and your needs. Choosing a proper bag the first thing to think about is what size of bag you would use because there are three different sizes of bags. The first size is what called the staff or tour golf bag. This is typically used by professional golf players who are hired by a company to promote their name or the brand of the golf club bag. The next is the golf cart bag. These are smaller compared to the staff or tour golf bags and are not meant for the player to carry it around rather just place them behind the golf cart. The third is the most versatile of the three because it can be either carried or placed behind the cart. This bag is called the golf stand carry bag. These bags can be rather heavy and can weigh about four pounds before putting in the equipment and golf clubs.\n\nDepending on the player and his or her needs, anyone of these bags can suit the player. To determine which type of golf club bag would suite the player’s needs he or she should first consider his or her playing style; if that player would usually walk or ride a cart. Then the player must think about if he or she usually carries his or her own bag or has a staff to carry the golf club bag. Basically, a player must first determine his or her golfing habits first; this is the most important factor to consider before purchasing a golf club bag. Once all these are settled then a player can purchase a suitable bag to compliment all of his or her golfing needs so that playing golf would be much easier and comfortable for the player.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Carting your Clubs Golf",
                 "Carting your Clubs Golf",
                 "Assets/13.jpg",
                 "Carting your Clubs Golf clubs are the primary equipment of anyone who plays golf. The objective in playing golf is to hit the ball from the teeing area, this act is called the drive, followed by successive strokes to get to the green and putt the ball into the hole called the cup.",
                 "Carting your Clubs Golf clubs are the primary equipment of anyone who plays golf. The objective in playing golf is to hit the ball from the teeing area, this act is called the drive, followed by successive strokes to get to the green and putt the ball into the hole called the cup. However, the journey from the teeing area or teeing ground to the green is filled with different parts and hazards. There are parts of a hole called rough and some are fairway. Hitting a ball from the rough is different from the fairway. Then you have sand traps and other hazards. These different situations need different clubs; this is why all golfers need a golf bag.\n\nGolf bags contain all the clubs and equipment of a golfer which he or she needs in order to complete the whole 9 hole or 18 hole course. These golf bags make life of a golfer easier and much more comfortable so that they can concentrate on playing and enjoying the game. However, choosing the right bag that suits your needs is important to attain this comfort. You have three kinds of golf bags you can choose from. The first is the golf staff bags. These golf staff bags contain everything a golfer would want out of their bags. These kinds of golf bags are typically used by professional golfers and are the largest and heaviest bags. So if you are an average player who cannot use the services of a caddy then this bag is not for you.\n\nNext we have the golf stand bags. These are the smallest type of golf bags available in the market today. They are carried around while walking to the next hole or shot and when placed down they have a spring loaded stand. This bag is best for those who carry their own bags however this does not have the same capacity of the other two bags and have less pockets and space. Last we have the golf cart bag.\n\nA golf cart bag is smaller compared to the golf staff bags but is relatively larger than golf stand bags. They are designed to be placed on the back of golf carts so the player does not need to carry it while moving on to the next course and does not need a caddy to bring it. A golf cart bag also has a lot of room and pockets for all your golf equipments. You can walk short distances from the cart with this bag and you do not need to strain yourself with carrying the bag to each hole thus being able to avoid shoulder or back injuries and problems. \n\nBasically the small things about your golfing habits would determine which bag is best for you. If you are not a player who joins tournaments or who plays in large courses then a golf cart bag is the best choice for you. Golf bags should compliment your golfing style so that you can enjoy the wonderful game of golf. ",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Famous golf courses",
                 "Famous golf courses",
                 "Assets/14.jpg",
                 "Famous golf courses: what makes them tick? Do you ever wonder what makes golf courses popular? Is it because of the world ranking pro golfers that tee off the greens or is the degree of difficulty of these courses that brings the best in every golfer.",
                 "Famous golf courses: what makes them tick? Do you ever wonder what makes golf courses popular? Is it because of the world ranking pro golfers that tee off the greens or is the degree of difficulty of these courses that brings the best in every golfer? Is it made famous by the natural scenery that makes golfing more enjoyable? Or is it because professional tournaments are always held in these country clubs? If you ask some of the renowned golf developers in the world, they would surely say that their creations are the best in the world. But is there a way for us to know which greens are more famous than others? And do these famous golf courses can also be considered as the best in the world?\n\nFor those that are not really familiar with the elite sport, there’s no telling whether a certain golf course is different from others. For them, every green is one and the same.  Yet even novice golfers would know the difference. The following are some of the well known golf courses that are famous for a lot of reasons.\n\nWhen people talk of golf destinations, Arkansas is always included in their discussion. The serene ambience and majestic beauty of the Natural State are simply the main factors why golf enthusiasts troop to Arkansas. They say the state’s natural beauty—a combination of hardwood forests, mountain lakes and water streams, and gentle rolling hills—complement the state’s numerous championship courses. These include The Legend at Arrowhead, Palm Valley, Scottsdale Silverado, Scottsdale C.C., and Pavilion Lakes, among others. And if the names of the famous designers like Robert Trent Jones Jr. and Edmund Ault were any indication, Arkansas’ golf courses naturally incorporated the sceneries of the state.\n\nThe search for the famous golf courses also takes us to Augusta, Georgia. Here is where Augusta National draws hordes of golf players every year since its opening in 1933, all eager to try its par 72 greens. Don’t be deceived by its seemingly easy first nine holes, because the game really starts on the 10th hole. This is where the fun begins. A lot of golf aficionados even consider the course as the world’s best known course because of the challenge it brings. \n\nThey say golf courses are only as good as the designers who created it. These words can only be the truth. In California, there is one golf course that was made famous by the creative genius of one man. Who would have thought that such beauty as The Pebble Beach in the Monterey Peninsula. Jack Neville is not really known in the golfing world. But The Pebble Beach proved to be a masterpiece that would bring his name to greater stature. Everyone was in awe seeing what he had done on that once desolate piece of property. Neville, being the humble man that he is, attributes the success of the country club to the natural landscape. According to him, all he had to do was to “put holes on the greens Of course, there are other famous golf courses out there. And the ones included here are just the tip of the iceberg. What’s certain is that there will be more, as golf landscapers will always be there to turn every isolated piece of land into a golf destination.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Master the Art Of Golf",
                 "Master the Art Of Golf",
                 "Assets/15.jpg",
                 "Master the Art Of Golf Club Swing In golf, the swing can make or break your game. This means that if you are swinging the wrong way, you may as well say goodbye to your dream of becoming the next Tiger Woods. The proper golf club swing can only be mastered if you spend a lot of time practicing.",
                 "Master the Art Of Golf Club Swing In golf, the swing can make or break your game. This means that if you are swinging the wrong way, you may as well say goodbye to your dream of becoming the next Tiger Woods. The proper golf club swing can only be mastered if you spend a lot of time practicing. It just doesn’t happen overnight. In fact, it takes weeks, months or years to even get to the point where you are very comfortable in swinging your club. Watching professionals do it on television gives you the notion that swinging is just a simple body movement that allows you to hit the ball and let it fly onto the air and land to the green. Well, they are professionals and they really make it look that easy. The truth is that a proper golf club swing entails the correct positioning of the hands on the golf club, the awareness of the swing plane and the spine angle, and the use of the correct golf club. When all these factors combined, you will be able to hit the ball and send it to where you want it to land. \n\nTo perfect the golf club swing, you must have a clear understanding of the swing plane. This is quite a difficult golf concept to understand. The simplest explanation is that swing plane is the area in which your golf club can swing in a circular arc and be able to move in that plane. If you can keep your clubhead within that plane, you are will be able to make good contact at every swing. This does not mean that you should force your club into the swing plane. What it does encourage you is to use it as a means to test your swings and see where you need to improve on. No two players will have the same swing. This is because of the difference in height and body type. What this says is that if you are short, you shouldn’t try to copy a tall player’s swing, and vice versa. \n\nMost golfers turn to their coaches and trainers to help in their golf club swing. This is a good way to get feedback on your swing faults. Other golfers make use of software that analyzes their swings and compare them side by side other players’ swings. The advantage of this is that you will be able to have an immediate feedback of your swing and can easily find ways to improver your performance on subsequent swings. Recording your golf club swings when you go on practice will help you pinpoint the main source of faults. At times, your swing will be faulty because you are not following the right hand and body position. It is also important to pay particular attention to the position of the ball vis-à-vis your position. The distance will play a pivotal role in the way you hit the ball. A good body turn can also tell so much about the ball impact. If you make use of the proper swing techniques and utilize the right golf club, you will be able to master the art and science of swinging.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Choosing The Right Golf Clubs",
                 "Choosing The Right Golf Clubs",
                 "Assets/16.jpg",
                 "Choosing The Right Golf Clubs Equipment Golf is one of the most popular individual sports in the world. It is a sport that combines several elements together in order to be good at the game. It requires physical, mental, and emotional toughness in order to be very effective at the sports.",
                 "Choosing The Right Golf Clubs Equipment Golf is one of the most popular individual sports in the world. It is a sport that combines several elements together in order to be good at the game. It requires physical, mental, and emotional toughness in order to be very effective at the sports. Absence of one factor may affect your game significantly. It is important to condition yourself physically and mentally before each game. But that is just one aspect of the preparation. It is also important to have the necessary golf clubs equipment.  If you are keen on starting your career as a golfer, you have to know what kinds of club you need to bring to the game. It will take many experiments and trials before you can finally decide what golf clubs to use. No one but you can tell which set of equipment would help improve your golf performance. \n\nThe standard golf clubs equipment set consists of 10 irons, 3 woods, and a putter. You have to keep in mind that this is the standard set many years ago. Some golfers stick to the standard because it works for them, but that doesn’t mean you will just blindly follow what others are doing. The goal is to improve your game and that entails choosing the best golf clubs equipment that fit your play style. Whether you are a beginner or an advanced player, you will realize that each game will have different situations wherein your skills in decision-making will be tested. \n\nGolf requires you to develop skills in swinging and putting. In both cases, you will have to make some calculations on how hard or soft you would hit the ball. There are many considerations when making your calculations. If you are hitting from a tee, you will have to be aware of the wind conditions as well as the path where you want the ball to drop. You have to take into account all the possible obstacles that the ball will encounter as it flies onto the air. You will have to ask yourself many times which golf clubs equipment will work at different situations. Wood may do the job when you want a distance shot, but if weather condition is not on your side, the ball may end up hitting the rough. The key to determining which golf clubs to use is to try them all at different scenarios. You have to consider the trajectory of the ball when you use a specific type of golf club. Iron normally has low trajectory, but it can be useful in situations where you need the ball to land at a short distance. If you feel that none of clubs work, then you might want to opt for a hybrid golf club. It combines the good stuff of the wood and iron and can be very useful in certain situations. It’s not enough to just use any kind of golf clubs equipment, you need to break them in and determine if they are the right ones for your skill level. ",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Perfect Golf",
                 "Perfect Golf",
                 "Assets/20.jpg",
                 "Golf competition is generally played for the lowest number of strokes by an individual, known simply as stroke play, or the lowest score on the most individual holes during a complete round by an individual or team, known as match play. Stroke play is the most commonly seen format at virtually all levels of play, although variations of match play such as skins games are also seen in televised events. Other forms of scoring also exist.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Why Golf Course Finders are Helpful?",
                 "Why Golf Course Finders are Helpful?",
                 "Assets/21.jpg",
                 "Why Golf Course Finders are Helpful Tired of visiting the same old golf course that is located several hours away from your home? Itching for new sights, sounds and sceneries? You may find yourself too lazy or too busy to even consider looking for a new golf course to golf in with your usual buddies.",
                 "Why Golf Course Finders are Helpful Tired of visiting the same old golf course that is located several hours away from your home? Itching for new sights, sounds and sceneries? You may find yourself too lazy or too busy to even consider looking for a new golf course to golf in with your usual buddies, but the task is much easier than you imagine it to be. If you think that you're stuck in your current golf course for the rest of your life, think again. Problems like that are what golf course finders are for! Since it is not a widely used term, many people ask what a golf course finder is all about. Well, one thing you should know is that they come in all shapes and sizes -- literally! They are essentially directories that you can find in golfing guides online or in magazines, newspapers and other golf-relate publications. They help locate all the golf courses listed in the country (or in the entire continent), no matter how obscure. They are simple, but effective tools that can be used by golf enthusiasts anywhere in the country. Anyone who finds themselves in a foreign state can easily use a finder to locate the nearest golf course, so that he or she can go there and golf during their free time! That is only one of many opportunities that arise when one uses a golf course finder.\n\nThey list everything about golf courses, including the exact addresses, websites, postal codes and miscellaneous information concerning courses and clubs in the country you live in. By searching your area, you can easily discover the hidden golf courses you never knew about. After that, you can immediately call up the golf course or golf club and ask about getting to play there. Finders help golfers all over the country experience new courses constantly. They also help bring customers to the most obscure golf courses that need the publicity. Without golf course finders, most golfers would have no choice but to stick to one course every time, not getting to experience things differently in any way. This lifestyle could easily become too boring for a golfer.\n\nBut where can you find a golf course finder? Easy: the internet is the easiest source for information. Many websites are dedicated to listing all the closest golf courses in your country and area. You can also subscribe to golfing magazines or purchase golf-related books, which often list the details of good, reliable golf courses where you can practice or even take lessons. Some finders are more detailed, and even list the golf courses' features, rates and facilities. Access your own golf course finder today, and you will never have to worry about getting bored of golf ever again. The constant change of scenery will not only improve your skills, but your mood as well. You will get to play in all sorts of locations and get to meet golf lovers similar to you. It's an offer you can't refuse, so what are you waiting for? ",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Good Golf Course Review",
                 "Good Golf Course Review",
                 "Assets/22.jpg",
                 "How to Write a Good Golf Course Review For any golf fanatic, it is perfectly natural to want to know as much as they can about golf courses. Like the saying says, one must be very familiar with the terrain or territory if they are striving to succeed.",
                 "How to Write a Good Golf Course Review For any golf fanatic, it is perfectly natural to want to know as much as they can about golf courses. Like the saying says, one must be very familiar with the terrain or territory if they are striving to succeed. This is why gold enthusiasts often take an interest in either writing or reading golf course reviews. Being updated on a golf course that they are planning to go to will make it easier for people to adjust, and they will know what to expect. It is also through golf course reviews that golfers can share their experiences in a certain golf course, and they will be informed of whether a certain golf course is worth visiting or not. How does one know what a good golf course review includes? In general, it should be informative and interesting, able to catch the eye of a reader and be comprehensive and up to date about the golf course being reviewed. The most important aspect of the review is that every fact should be truthful, and as much as possible, the review should be as objective and non-biased as possible. \n\nFirst of all, a proper golf course review should state the value of the golf course. It need not be specific about the costs and prices of the amenities and golf course facilities, but it should be honest about whether usage is overpriced. Secondly, it is good to be very descriptive when it comes to the golf course itself. The grass matters, and should be rated based on its quality, color (for aesthetic appeal) and even texture, for the sake of other golf afficionados. Also, one might want to specificy whether the course is family friendly or not, for casual golfers who might be going there as part of a family vacation. It is recommended that the important amenities and features of the golf course be specified. \n\nIt is integral that the service of the golf course be described in full detail as well. The caddies' performances and the qualities of rented golf carts, golf clubs and golf balls are worth mentioning. Any unique qualities of the golf course are helpful in attracting other people. Some readers might even want to know more about the course's scenery, pros and cons in general, or details about any hotel or resort that the golf course is part of. Lastly, the accomodation details, location and contact numbers for the golf course should be mentioned if one really wants to encourage others to try it out.\n\nAnyone can write their own golf course review after visiting one, just like how any person can do research and read reviews for other golf courses that they plan to visit. It is by reading such reviews that golfers can expand their horizons and be exposed to different places. By trying out recommended courses, any golfer can also heighten their golfing experience. Seemingly hidden but beautiful golf courses can be shared with the rest of the world, and people can compare and contrast their favorite places to golf in. ",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Analyze Your Swing",
                 "Analyze Your Swing",
                 "Assets/23.jpg",
                 "Analyze Your Swing With A Gold Swing Software Golf is probably one of the best sports to engage in if you are looking for a total body exercise.",
                 "Analyze Your Swing With A Gold Swing Software Golf is probably one of the best sports to engage in if you are looking for a total body exercise. It is not as strenuous as a gym workout and it most certainly is not violent as boxing or American football, but what it does is to make your mind, body, and emotions work in unison.  It’s one of the few sports that test your mental and physical skills. It encourages you to have a lot of patience and lets you keep your emotions in check. The intricate balance of those factors can tell so much about your game performance. Of course, you also need to have the proper golf clubs to make things happen. When you feel that you are physically, mentally, and emotionally prepared to play golf, then the next thing to do is to practice your swing. The swing is what separates the men from the boys. It may look easy when you watch players on television, but when you’re finally there at the golf course; you will find yourself wondering why you can’t swing like Tiger Woods.\n\nSome golf trainers say that there are golfers who have the innate ability to swing like a professional. It takes little to no effort on their part to make that perfect swing. The problem is that not everyone has that ability. That’s not to say there’s no hope for ordinary people like us. In fact, swinging is a skill and just like any other skill, it can be learned, developed, and improved. It doesn’t matter if you are a beginner, amateur, or advanced golfer; the truth remains, you need to practice, practice, and practice some more. Just keep in mind that everytime you practice, you need some kind of feedback from your trainer or coach. He or she should be able to evaluate your swing and let you know what you are doing right and what you are doing wrong. Unfortunately, not many people can afford to have a trainer each time they play. There are several ways to address this problem and one of those is to make use of a golf swing software. What it does is to record and analyze your swing so that you can improve your game. It’s like having a personal trainer but with more accurate feedback. \n\nA golf swing software will allow you to analyze your swing and even compare it side by side other people’s swing. It has comparison and drawing tools that will give you instant feedback. It’s also a great way to see how professionals do it and you can mimic what they do. Having an immediate visual feedback will help you improve on your game by avoiding swing habits that can ruin your game. With a golf swing software, you will have a better understanding of the swing plane. This way, you will know where the golf club should remain during takeaway up until impact. You will also learn where to properly position your hands in order to have a full swing. \n\nBelieve it or not, even professionals make you use of golf swing software to improve their game. So, there’s no need to feel silly about recording yourself while swinging. The best part of using a golf swing software is that you can see yourself in slow and reverse motion, so you will see the areas you need to work on.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Practice With Your Golf",
                 "Practice With Your Golf",
                 "Assets/24.jpg",
                 "Practice With Your Golf Swing Trainer Many golfers believe that the perfect swing can be achieved with determination, patience, and a whole lot of practice. If you are seriously considering a career as a golfer, you must be physically, mentally, and emotionally prepared for any possible situation while in the golf course.",
                 "Practice With Your Golf Swing Trainer Many golfers believe that the perfect swing can be achieved with determination, patience, and a whole lot of practice. If you are seriously considering a career as a golfer, you must be physically, mentally, and emotionally prepared for any possible situation while in the golf course. Having the right set of golf clubs will definitely help improve your chances of winning a tournament or two. But the very core of the sports is the golf swing. Many people believe that the swing can make or break your game. The swing signals the start of the game and the moment your golf club hits the ball, you can immediately have an idea of how the game can turn out. Needless to say, a bad swing can ruin your game. That is why amateur and professional golfers are going through some great lengths just to master the art of the perfect swing. However, not everyone believes that there is no such thing as a perfect swing. There may be a germ of truth in this belief because if you watch golfers in tournaments, you will see that each of them has different swing. Even though some of their swings look awkward, the golfers are able to position the ball to the spot they wanted it to land. So, you might be able to conclude that a perfect swing will vary depending on the technique of the golfer. \n\nOne of the ways to improve your swing is to get a golf swing trainer.  Getting a coach or trainer to give you regular feedbacks will help you realize what you are doing wrong and what you are doing right. Once you figure out your bad swing habits, you can find ways to get rid of them and make use of the technique that you are most comfortable in. Not many people would believe that a great swing can be made in the most natural and most comfortable way. A golf swing trainer will tell you that a swing is simple to do but is made complicated by being overly conscious of your hand and body positions. This kind of attitude towards the swing will not only make the shot awkward, it will also make you feel less confident on each swing. Having the confidence at the very start will help get rid of negative thoughts about the act of swinging. Remember, mental toughness is also necessary in the game, so try not to entertain thoughts that can affect your game.\n\nThere are times when a coach may not be necessary especially if you are playing golf just for fun or just as a pastime. This should not be a problem because a golf swing trainer can also be a software. All you need is to record yourself while practicing your swings and the golf swing trainer will help you analyze your performance. It will give you instant feedbacks by showing you diagrams that pinpoints the areas where you need to improve on. There are also comparison tools that will show you how you fair against other players. Having a golf swing trainer is like having a coach with accurate feedback and calculations. It’s the next best thing to having the perfect swing.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Hybrid Golf Club",
                 "Hybrid Golf Club",
                 "Assets/25.jpg",
                 "Swing Your Way to Victory! Playing golf involves an intricate balance of skills, equipment, weather, and mood. Without sufficient preparation, your game can be a disaster in the making. Whether you are participating in a tournament or just bonding with your mates, the game of golf requires you to be in a competitive state of mind.",
                 "Hybrid Golf Club: Swing Your Way to Victory! Playing golf involves an intricate balance of skills, equipment, weather, and mood. Without sufficient preparation, your game can be a disaster in the making. Whether you are participating in a tournament or just bonding with your mates, the game of golf requires you to be in a competitive state of mind. Winning is not everything, but in any sports, it is the ultimate goal. If you are able to manage your emotions and you are quite confident in your swinging and putting skills, then you have to focus your attention on your equipment. Not many players are aware of the subtle differences of various types of golf clubs. Knowing these differences can greatly improve your performance. The feel of a golf club is not enough indication that you are using the right kind of club. You should also take into consideration how you can hit the ball better in different situations and conditions. This could mean switching to different golf clubs depending on where the ball landed. Sometimes it can be a pain to constantly switch to different golf clubs. \n\nBack in the days, golfers only have to choose between an iron or wood, depending on preference and the level of expertise. Using either type of club exclusively can have some disadvantages. An iron club may have added power on every swing but low trajectory can be a problem. On the other hand, a wood has longer shaft that will require different swing techniques. In most cases, a ball hit by wood tends to skim over taller grasses instead of cutting to them. Advancement in technology has made it possible for manufacturers to create hybrid golf clubs.\n\nHybrid golf clubs are designed to hit golf balls better under certain situations or weather conditions. They basically combine all the best features of wood and iron. As such, they are able to address the problem of having to constantly switch clubs mid-game. This type of club not only benefits the professional golfers, it also helps improve the game of amateurs as well as enthusiasts. The first time you hold a hybrid golf club, you will immediately see the similarity to a fairway wood. The only difference is that it has a slight convex face. When you hit the ball, you will feel the “trampoline effect”, which causes the clubface to deform slightly and revert to its original shape. This effect is said to increase the impulse applied to the golf ball, which allows for higher trajectory but with more control. \n\nUsing hybrid golf clubs is actually getting the best of both worlds. They are very versatile that is why they are considered as utility golf clubs. You can use them to hit the ball onto the green, down the fairway, out of the rough, or into the air. They do a great job in various golf situations. Professional golfers swear by the versatility of the hybrid golf clubs.  They are not out to replace your lucky golf club, but there will come a time when you will find yourself in the middle of a tough golf situation. Hybrid golf clubs will be there to rescue you.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Local Golf Courses",
                 "Local Golf Courses",
                 "Assets/26.jpg",
                 "Among the best in the world Some people think that golf is an elite sport and that playing it would mean that they have to spend a lot of money. After all, rich businessmen and top executives are the ones who play rounds of golf during their free time.",
                 "Local Golf Courses: Among the best in the world Some people think that golf is an elite sport and that playing it would mean that they have to spend a lot of money. After all, rich businessmen and top executives are the ones who play rounds of golf during their free time. It is also a common practice for them to meet with potential clients during a golf vacation. Yet one need not spend a lot or even go to different far-off places to play this game. In fact there are a lot of local golf courses that you can go to should you feel the need to try that golf swing you’ve been practicing for a long time. The US is home to great golf courses—some of them are even favorite venues for pro tours. Perhaps it is the variety of physical appearance and level of difficulty that made these golf courses famous not only here but in the international golfing community as well.\n\nA pro golfer will never exclude the Pebble Beach Golf Links in his or her top selections. Dating as far back as 1919, this critically acclaimed link course has become the hub where classic meets modern in landscape and facility. In fact, the golf magazine US’ Digest Golf included it in its top ranking golf courses in 2005. But what’s set this golf course apart is probably its awesome terrain or the rugged coastline that marks the boundaries of its greens. Jack Neville, the designer of this golf course, didn’t need to do much and just used the natural landscape to create a golf course like no other. Because of its innate beauty and world class facilities, The Pebble Beach golf course has been hosting some of the major golf tournaments in the country—one of which is the US Open Championships. Equally popular is the Pinehurst Resort and Country Club. One of the oldest golf courses in the country, Pinehurst is filled with proud traditions and rich history. And for those who prefer to stay here will be surprised that the 2,000-acre property has a total of eight different golf courses with different levels of difficulty. You’ll find everything here. You can take your whole family because there are great amenities that will cater to their needs while you play a round of golf. No wonder it is included in almost every best local golf courses list because it provides a total getaway for golfers and their friends and families. Guests will enjoy their stay whether they are playing golf under the sun or simply relaxing while sipping a drink or two.\n\nNow, who said that Las Vegas is all about casinos? There’s one that has been a favorite destination among golf enthusiasts and it is Shadow Creek.  Actually if you are staying in an MGM property, you’ll get to play for free in this golf course as it is often included as a package. But anyone can book for a round of golf at Shadow Creek while you are in Vegas. Once the reservation has been made, you’ll be surprised that a limousine will pick you up whichever hotel in Vegas you are staying. It’s just one of the many perks of playing in Shadow Creek.\n\nPlay in one of these local golf courses and you’ll surely feel like a pro. Despite of the game’s elite reputation, everyone can play the game. All it takes is guts that you can be as good as the pro golfers.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
