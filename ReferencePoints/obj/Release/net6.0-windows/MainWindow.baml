   M S B A M L   `   `   `  ���� J  FReferencePoints, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ReferencePointsReferencePoints  G(clr-namespace:ReferencePoints.ViewModelsReferencePoints.ViewModels  G(clr-namespace:ReferencePoints.ConvertersReferencePoints.Converters       ReferencePoints.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:ReferencePoints     7vm(clr-namespace:ReferencePoints.ViewModels     ?
converters(clr-namespace:ReferencePoints.Converters       � Title$  Reference Points�� � WindowState$ Normal=� � 	Resources :   4ReferencePoints.Converters.BoolToVisibilityConverter   BoolToVisibilityConverterB   <ReferencePoints.Converters.InvertedBoolToVisibilityConverter % !InvertedBoolToVisibilityConverter �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� Padding	 �� CornerRadius
 J� Opacity �� Triggers P� 
SourceName P� Property P� Value �� 
TargetName �� Triggers��%�  &	        &	      '�� 
         �� " ��.���  � �� �#EFEFEF��  � �� �� " ��.R��� $ 
mainBorder��$ 	10,5,10,5q�
��� � �$��1q�$	 5��#��z��._��� #��z��#
 z
  P� $ 
mainBorder�� �@J�IsMouseOver$
 True��. ��� $ 
mainBorder�� � ��
 ����� P�  �@J�IsMouseOver$
 True��. ���  � �� �#d4d4d4P�  � {�$ False��. ���  �@J�Opacity$	 0.7Q�.���  � RowDefinitions �� $��1* ��� $��4* �.+���  �� Orientation$ 
Horizontal=�$��50��.+��� $��Upload Image��$��10q����� *UploadImageCommand+�� $��Convert to Grayscale��$��10q����� *ConvertToGrayscaleCommand+�� $��Analyze Image��$��10q����� *AnalyzeImageCommand+�� $��Save Result��$��10q����� *ExportResultCommand+�� $ 
Horizontal=�$��10q�.+��� $��	Comparer:��$��Center=�$��0 0 5 0q��� $��Center=�$��Center=�$��25��$��50������ *
Comparer+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ 	LostFocus=��� $��1��$
��auto��$
��auto��$��10q����#F6F6DB
��� � �$��1q�._��� $��5q����� *	ImagePath+$ PropertyChanged=��� $��2�����#dedede$	
 0.4Q�$	 5��y��� *IsBusy+$ PropertyChanged=�$ OneWay=� �� 	Converter# [  �� $��2��$��Center=�$��Center=�y��� *IsBusy+$ PropertyChanged=�$ OneWay=�# [  .+��� $��Processing...��$��Center=�$��Center=��  � IsIndeterminate �� *IsBusy+$ PropertyChanged=�$ OneWay=�