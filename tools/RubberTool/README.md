# RubberTool

Утилита для "эластичной трансформации" растровых изображений.

Прицип основан на построении триангуляционной сетки (TIN) на базе ключевых точек между двумя поверхностями.

Сетка строится триангуляцией Делоне, которая не даёт формироваться "тонким" треугольникам.

Затем пиксели одного изображения подгоняются треугольник к треугольнику другого изображения.

## Credits

- Делоне за алгоритм
- Вики, статья "Способ резинового листа"
- Иконка by Patrick Orlando
- corrmap.com
