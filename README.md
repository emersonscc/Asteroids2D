# Asteroids 2D – Unity

Um clone do clássico **Asteroids** (com naves inimigas, ranking e menu) desenvolvido em **Unity 2D**. O projeto foi construído em etapas para fins de estudo, mantendo a jogabilidade simples e clara.

## Links 
- Executável do jogo (Asteroids2D/Run/My Project.exe) : https://drive.google.com/file/d/1nlgZb8hpeQULTmuKf5vvm-MfyDLlR0v7/view?usp=sharing
- Vídeo Demo do jogo: https://drive.google.com/file/d/1i3C16e-Wz5xzDj_j1a1AQzMl9Q9ylEws/view?usp=sharing

## Sumário
- [Como Jogar](#como-jogar)
- [Objetivo & Condições de Vitória](#objetivo--condições-de-vitória)
- [Pontuação](#pontuação)
- [Dificuldade (Fácil/Difícil)](#dificuldade-fácildifícil)
- [Regras de Dano e Destruição](#regras-de-dano-e-destruição)
- [Leaderboard (Ranking)](#leaderboard-ranking)
- [Controles](#controles)
- [Cenas & Fluxo](#cenas--fluxo)
- [Como Executar no Editor](#como-executar-no-editor)
- [Como Gerar Executável](#como-gerar-executável)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Configurações Importantes](#configurações-importantes)

---

## Como Jogar
- Você controla uma nave em um campo espacial 2D.
- **Acerte e destrua** asteroides e **desvie** de tiros e inimigos.
- **Derrote inimigos** para somar pontos e cumprir requisitos do ranking.
- Alcance o **planeta no topo do mapa** para **vencer** a partida.

## Objetivo & Condições de Vitória
- O objetivo principal é **alcançar o planeta** (um *GoalZone* com BoxCollider2D “Is Trigger”).
- Ao encostar na *GoalZone*, o jogo retorna ao menu.
- **Só entra no ranking** quem:
  1) **Vence** (chega ao planeta), e  
  2) **Destrói ao menos N inimigos** (N depende da dificuldade; ver abaixo).

## Pontuação
- **+50 pontos** por **inimigo** destruído (valor padrão).  
- (Se você ajustar `scoreOnKill` nos prefabs/scripts do inimigo, o HUD refletirá automaticamente.)  
- O **HUD** exibe o placar atual durante a partida.

## Dificuldade (Fácil/Difícil)
No **menu principal** existem dois botões:

- **Fácil:** precisa destruir **pelo menos 5** inimigos para o score aparecer no ranking.  
- **Difícil:** precisa destruir **pelo menos 10** inimigos.

> A escolha da dificuldade é salva em `PlayerPrefs` e lida pelo `GameManager` no início da partida.

## Regras de Dano e Destruição
- **Nave do jogador**
  - **Tiros inimigos:** ao ser atingida **30 vezes** no total, a nave é destruída (reinicia/volta ao menu conforme a lógica do jogo).
  - **Colisão com asteroides:** após **5 colisões** a nave é destruída.
  - **Colisão com inimigos:** após **5 colisões** a nave é destruída.
- **Naves inimigas**
  - Precisam de **3 tiros** do jogador para serem destruídas (valor definido no prefab).
- **Asteroides**
  - **Grandes** se dividem em **2 médios** quando destruídos.
  - **Médios** se dividem em **2 pequenos** quando destruídos.
  - **Pequenos** somem quando destruídos.

> Explosões visuais aparecem por ~1 segundo quando jogador ou inimigos são destruídos.

## Leaderboard (Ranking)
- O ranking é exibido **no menu**.
- Para registrar pontuação:
  1) Preencha um **nome válido** (mín. 2 caracteres, letras/números/espaços).
  2) **Vença** a partida (alcance o planeta).
  3) **Destrua** ao menos **5 inimigos** (Fácil) **ou 10** (Difícil).
- **Resetar ranking:** no **menu**, pressione **R** (apaga os dados do ranking).  
- **Sair do jogo (menu):** `Esc` ou `F10`.
- Os dados são salvos em `PlayerPrefs` (chave padrão `leaderboard_v1`).

## Controles
- **Mover/Rotacionar:** `←/→` para rotacionar.
- **Acelerar:** `↑`.
- **Desacelerar:** `↧`.
- **Atirar:** `Espaço`.

> As teclas podem ter sido adaptadas/localmente; ajuste conforme seu `PlayerController` e `MenuManager`.

## Cenas & Fluxo
- **MainMenu (índice 0)** – menu inicial com:
  - Input de nome do jogador
  - Botões: **Fácil** e **Difícil**
  - Leaderboard sempre visível
  - Atalhos: **R** (reset ranking), **Esc/F10** (sair do jogo)
- **Game** – cena principal de jogo (nome configurável em `MenuManager.gameSceneName`).

Fluxo: **MainMenu → (Escolhe dificuldade) → Game → Vitória/Morte → MainMenu**.

## Como Executar no Editor
1. Abra o projeto no Unity.  
2. Garanta que a cena **MainMenu** está aberta e incluída no **Build Settings** (índice 0).  
3. **Play** para testar.

Se o Input Field não aceitar digitação:
- *Project Settings ▸ Player ▸ Active Input Handling* = **Both** (ou configure o EventSystem para o input que você utiliza).

## Como Gerar Executável
1. **File ▸ Build Settings…**
   - Em **Scenes In Build**, deixe **MainMenu** no topo, depois **Game**.
   - **Platform:** *Windows* (x86_64) e **Switch Platform**.
2. **Player Settings…**
   - Defina **Product Name**, **Company Name** e (opcional) **Icon**.
   - **Other Settings:** *Scripting Backend* (Mono para builds rápidas ou IL2CPP para distribuição).
   - **Active Input Handling:** **Both** (recomendado para compatibilidade).
3. **Build** (ou **Build and Run**). Compacte a pasta gerada para distribuir.

## Estrutura do Projeto
```
Assets/
  Scripts/
    GameManager.cs
    MenuManager.cs
    LeaderboardManager.cs
    PlayerController.cs
    Enemy.cs
    EnemySpawner.cs
    Bullet.cs
    Asteroid.cs
    GoalZone.cs
    ...
  Prefabs/
    Player.prefab
    Enemy.prefab
    AsteroidLarge/Medium/Small.prefab
    Bullet.prefab
    Explosion.prefab
    ...
  Scenes/
    MainMenu.unity
    Game.unity   (ou SampleScene / nome definido em MenuManager)
```
## Configurações Importantes
- **Build Settings:** certifique-se de que **MainMenu** é a cena 0.
- **MenuManager:** `gameSceneName` deve corresponder ao nome da cena do jogo.
- **UI (Legacy):** Canvas com **Graphic Raycaster** e EventSystem **Standalone Input Module** (ou Input System UI compatível).
- **PlayerPrefs:** dados de ranking e dificuldade são persistidos; para limpar tudo use `PlayerPrefs.DeleteAll()` (apenas para testes).
