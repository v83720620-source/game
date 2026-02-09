using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlumpGame.UI.Lobby
{
    /// <summary>
    /// Представляет одного игрока в списке лобби.
    /// </summary>
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _pingText;
        [SerializeField] private Image _teamColorImage;
        
        public void Setup(string playerName, int level, int ping, int team)
        {
            if (_playerNameText != null)
                _playerNameText.text = playerName;
            
            if (_levelText != null)
                _levelText.text = $"Lvl {level}";
            
            if (_pingText != null)
            {
                _pingText.text = $"{ping}ms";
                
                // Цвет пинга (зелёный = хороший, жёлтый = средний, красный = плохой)
                if (ping < 50)
                    _pingText.color = Color.green;
                else if (ping < 100)
                    _pingText.color = Color.yellow;
                else
                    _pingText.color = Color.red;
            }
            
            if (_teamColorImage != null)
            {
                _teamColorImage.color = team == 1 ? new Color(0.2f, 0.4f, 1f) : new Color(1f, 0.2f, 0.2f);
            }
            
            // TODO: Загрузить аватар игрока
        }
    }
}
