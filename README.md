# MedLab-Mediator
By Nogah Melamed Cohen.

Design: MVC
Model - Model.cs, StateFunctionCalculator.cs
View - MainWindow.xaml, Results.xaml
Controller - MainWindow.xaml.cs, Results.xaml.cs

Calculate state function abstraction fot two concepts and function:
Algorithm (DB1, DB2, func):

1. sort DB1 by StartTime
2. sort DB2 by EndTime
//StartTime=StartTime-GoodBefore, EndTime=EndTime+GoodAfter
3. int j=0, string s=""
4. for i=0 ; i<DB1.length ; i++
  4.1. d1=DB1[i], d2=DB2[j]
  4.2. while (d1.StartTime>d2.EndTime) //find the first item in DB2 that can be overlap with d1
    4.2.1. j++
    4.2.2. d2=DB2[j]
  4.3. int index=j
  4.4. while(index<DB2.length && d2.StartTime<d1.EndTime)
    4.4.1. if there is overlap
      4.4.1.1. s += overlapInfo
    4.4.2. index++
    4.4.3. d2=DB2[index]
5. return s

