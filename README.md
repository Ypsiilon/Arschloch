# Brainstorming Daten

## Daten, welche den KIs zugaenglich sein sollen.
- PlayerOrder
  - TurnZero bool
  - Index(Array)
  - GewinnIndex(Array)
- discardPile(List\<Karte\>)
  - TopKarten
- Zug
  - Karten(Array\<Karte\>)
- Eigene Karten(Array\<Karte\>)
  - Abgegebene Karten(Array\<Karte\>)

## Karte
- Farbe(int)
- Wert(int)

## MatchHierarchy
- Life
- Olympiade
- Tournament
- Match (multiple ..)
- Round (multiple Turns)
- Turn (Turn of one Player laying down its cards)