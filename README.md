# Brainstorming Daten

## Daten, welche den KIs zugaenglich sein sollen.
### Input
- PlayerOrder
  - TurnZero ``Boolean``
  - Index ``Array``
  - GewinnIndex ``Array``
- discardPile ``List<Karte>``
  - TopKarten `List<Karte>`
### Output
- Zug
  - Karten ``List<Karte>``
- Eigene Karten ``List<Karte>``
  - Abgegebene Karten `List<Karte>`

## Karte
- Farbe ``int``
- Wert ``int``

## MatchHierarchy
- Life
- Olympiade
- Tournament
- Match (multiple ..)
- Round (multiple Turns)
- Turn (Turn of one Player laying down its cards)