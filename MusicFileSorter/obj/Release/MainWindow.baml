   M S B A M L   `   `   `  ����   MusicFileSorter     MusicFileSorter.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Music File Sorter��$	��350��$	��525��.���  � RowDefinitions �� $��* ��� $��30 �.+�]� $��0��.+�o� $	��Top=�.@�m� $
��File��.@�m� -   $
��Open��m� $��Recent Items��.@�m� $��1��m� $��2��m� $��3��m� -   $
��Exit��m� $
��View��m� $
��Edit��� $	��Top=�$��0�� � ColumnDefinitions �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto � �� $
��auto ��� $
��auto �.+��� $��Button��$��0���� $��Button��$��1���� $��Label��$��0��$��1���� $��Label��$��1��$��1��� $
��Left=�$��1�� �� $��60 ��� $
��auto ��� $
��auto � �� $
��auto ��� $
��auto ��� $
��auto �.+��� -   $
��Scan��$��0��$��0��$��40��$��2q��� -   $
��Save��$��1��$��0��$��40��$��2q��� -   $��Delete��$��2��$��0��$��40��$��2q�� $��Right=�$��1�� �� $��2* ��� $��80 � �� $��30 ��� $	��10* �.+��� -    ��Name$ 	txtEditor��$��0��$��0��$*��$Enter folder path or click Browse...��$��18(�$��Italic$�$��2q��� -   $��	Browse...��$��0��$��1��$��2q�&   System.Windows.Controls.DataGrid  -     Name$ MusicDataFile��$��1��$��2��$��2q�� $��1�� �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $��* �.+��� $��Useful Info >>>��$��0��$
��Bold"��� $��Files found:��$��1���� -	    ��Name$ NumberOfFiles��$	��n/a��$��2���� $��Subfolders scanned:��$��3���� -
   $ NumberOfSubfolders��$	��n/a��$��4���� $��Scan Duration(sec):��$��5���� -   $ ScanDurationInSec��$	��n/a��$��6��