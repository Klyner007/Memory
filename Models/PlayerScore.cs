using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Memory.Annotations;

namespace Memory.Models
{
    class PlayerScore : INotifyPropertyChanged
    {
        private int totalScorePlayerOne;
        private int gameScorePlayerOne;
        private int timePlayerOne;
        private int wrongChoicesPlayerOne;
        private int totalScorePlayerTwo;
        private int gameScorePlayerTwo;
        private int timePlayerTwo;
        private int wrongChoicesPlayerTwo;

        public int TotalScorePlayerTwo
        {
            get
            {
                return totalScorePlayerTwo;
            }
            set
            {
                totalScorePlayerTwo = value;
                OnPropertyChanged();
            }
        }

        public int GameScorePlayerTwo
        {
            get
            {
                return gameScorePlayerTwo;
            }
            set
            {
                gameScorePlayerTwo = value;
                OnPropertyChanged();
            }
        }

        public int TimePlayerTwo
        {
            get
            {
                return timePlayerTwo;
            }
            set
            {
                timePlayerTwo = value;
                OnPropertyChanged();
            }
        }

        public int WrongChoicesPlayerTwo
        {
            get
            {
                return wrongChoicesPlayerTwo;
            }
            set
            {
                wrongChoicesPlayerTwo = value;
                OnPropertyChanged();
            }
        }

        public int TotalScorePlayerOne
        {
            get
            {
                return totalScorePlayerOne;
            }
            set
            {
                totalScorePlayerOne = value;
                OnPropertyChanged();
            }
        }

        public int GameScorePlayerOne
        {
            get
            {
                return gameScorePlayerOne;
            }
            set
            {
                gameScorePlayerOne = value;
                OnPropertyChanged();
            }
        }

        public int TimePlayerOne
        {
            get
            {
                return timePlayerOne;
            }
            set
            {
                timePlayerOne = value;
                OnPropertyChanged();
            }
        }

        public int WrongChoicesPlayerOne
        {
            get
            {
                return wrongChoicesPlayerOne;
            }
            set
            {
                wrongChoicesPlayerOne = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
