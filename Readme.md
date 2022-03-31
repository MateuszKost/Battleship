Cały program został zaprogramowany zgodnie z zasadami przedstawionymi w https://en.wikipedia.org/wiki/Battleship_(game).

Został on podzielony na kilka warstw.

- warstwa BattleShip, która jest głównym projektem gdzie znajdują się dwa algorytmy: 
	- pierwszy odpowiedzialny za tworzenie graczy wraz z ich statkami i punktami do nich należącymi oraz swoją mapą, 
	na której znajdują się statki i mapą przeciwnika.
	- drugi odpowiedzialny za cały gameplay. Każda z funkcji widnieje dwa razy. Na takie rozwiązanie zdecydowałem się z powodu
	zwielokrotnionej ilości powtarzających się ifów oraz wyniknęło to podczas refaktoryzacji, co znacznie uprościło logikę.

- warstwa CommonObjects, która zawiera wszystkie typy wyliczeniowe oraz wartości dla takich zmiennych jak np domyślna wielkość dla 
własnej mapy, jak i przeciwnika.

- warstwa MainObjects, w której znajdują się klasy odpowiedzialne za reprezentacje Gracza, jego statków i punktów.


Ze względu na to, że algorytm miał reprezentować symulację gry, każdy strzał w nowy punkt bez poprzedniego trafienia jest randomowy,
natomiast w momencie gdy trafimy, utworzony zostanie słownik z punktami, która znajdują się na około punktu trafionego, następnie
właśnie z  nich zostanie wylosowany kolejny punkt.

Program został zabezpieczony przed wszystkimi możliwymi błędami oraz rozpatruje każdą ewentualność strzału.

Zgodnie z zasadami, każdy gracz ma 5 statków różnej wielkości, a domyślna wielkość mapy jest równa 10x10


W CommonObjects znajdziemy 3 rodzaje typów wyliczeniowych wykorzystywanych w programie:
	- IndexType
	- ShipType
	- PointStatus

Pierwszy z nich przechowuje informacje o indexach punktów jakie powinny zostać dodane.
ShipType przechowuje informacje o typach statków, jak i ich wielkości wykorzystywanych w programie.
PointStatus jak sama nazwa wskazuje informuje nas o aktualnym statusie punktu, czy został trafiony, czy jest wolny, itp.

