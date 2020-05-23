﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyHoSoSucKhoe.Models
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="QLHSSK")]
	public partial class SQLServerDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertthongbao(thongbao instance);
    partial void Updatethongbao(thongbao instance);
    partial void Deletethongbao(thongbao instance);
    partial void Insertnguoidung(nguoidung instance);
    partial void Updatenguoidung(nguoidung instance);
    partial void Deletenguoidung(nguoidung instance);
    partial void Insertdmxa(dmxa instance);
    partial void Updatedmxa(dmxa instance);
    partial void Deletedmxa(dmxa instance);
    partial void Insertdmtinh(dmtinh instance);
    partial void Updatedmtinh(dmtinh instance);
    partial void Deletedmtinh(dmtinh instance);
    partial void Insertdmhuyen(dmhuyen instance);
    partial void Updatedmhuyen(dmhuyen instance);
    partial void Deletedmhuyen(dmhuyen instance);
    partial void Insertdmdantoc(dmdantoc instance);
    partial void Updatedmdantoc(dmdantoc instance);
    partial void Deletedmdantoc(dmdantoc instance);
    partial void Insertdmbenhvien(dmbenhvien instance);
    partial void Updatedmbenhvien(dmbenhvien instance);
    partial void Deletedmbenhvien(dmbenhvien instance);
    #endregion
		
		public SQLServerDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["QLHSSKConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SQLServerDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SQLServerDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SQLServerDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SQLServerDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<thongbao> thongbaos
		{
			get
			{
				return this.GetTable<thongbao>();
			}
		}
		
		public System.Data.Linq.Table<nguoidung> nguoidungs
		{
			get
			{
				return this.GetTable<nguoidung>();
			}
		}
		
		public System.Data.Linq.Table<dmxa> dmxas
		{
			get
			{
				return this.GetTable<dmxa>();
			}
		}
		
		public System.Data.Linq.Table<dmtinh> dmtinhs
		{
			get
			{
				return this.GetTable<dmtinh>();
			}
		}
		
		public System.Data.Linq.Table<dmhuyen> dmhuyens
		{
			get
			{
				return this.GetTable<dmhuyen>();
			}
		}
		
		public System.Data.Linq.Table<dmdantoc> dmdantocs
		{
			get
			{
				return this.GetTable<dmdantoc>();
			}
		}
		
		public System.Data.Linq.Table<dmbenhvien> dmbenhviens
		{
			get
			{
				return this.GetTable<dmbenhvien>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.thongbao")]
	public partial class thongbao : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _tu;
		
		private string _den;
		
		private System.DateTime _times;
		
		private System.Nullable<System.DateTime> _hansudung;
		
		private int _dadoc;
		
		private string _noidung;
		
		private string _chitiet;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OntuChanging(string value);
    partial void OntuChanged();
    partial void OndenChanging(string value);
    partial void OndenChanged();
    partial void OntimesChanging(System.DateTime value);
    partial void OntimesChanged();
    partial void OnhansudungChanging(System.Nullable<System.DateTime> value);
    partial void OnhansudungChanged();
    partial void OndadocChanging(int value);
    partial void OndadocChanged();
    partial void OnnoidungChanging(string value);
    partial void OnnoidungChanged();
    partial void OnchitietChanging(string value);
    partial void OnchitietChanged();
    #endregion
		
		public thongbao()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tu", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string tu
		{
			get
			{
				return this._tu;
			}
			set
			{
				if ((this._tu != value))
				{
					this.OntuChanging(value);
					this.SendPropertyChanging();
					this._tu = value;
					this.SendPropertyChanged("tu");
					this.OntuChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_den", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string den
		{
			get
			{
				return this._den;
			}
			set
			{
				if ((this._den != value))
				{
					this.OndenChanging(value);
					this.SendPropertyChanging();
					this._den = value;
					this.SendPropertyChanged("den");
					this.OndenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_times", DbType="DateTime NOT NULL")]
		public System.DateTime times
		{
			get
			{
				return this._times;
			}
			set
			{
				if ((this._times != value))
				{
					this.OntimesChanging(value);
					this.SendPropertyChanging();
					this._times = value;
					this.SendPropertyChanged("times");
					this.OntimesChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_hansudung", DbType="DateTime")]
		public System.Nullable<System.DateTime> hansudung
		{
			get
			{
				return this._hansudung;
			}
			set
			{
				if ((this._hansudung != value))
				{
					this.OnhansudungChanging(value);
					this.SendPropertyChanging();
					this._hansudung = value;
					this.SendPropertyChanged("hansudung");
					this.OnhansudungChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_dadoc", DbType="Int NOT NULL")]
		public int dadoc
		{
			get
			{
				return this._dadoc;
			}
			set
			{
				if ((this._dadoc != value))
				{
					this.OndadocChanging(value);
					this.SendPropertyChanging();
					this._dadoc = value;
					this.SendPropertyChanged("dadoc");
					this.OndadocChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_noidung", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string noidung
		{
			get
			{
				return this._noidung;
			}
			set
			{
				if ((this._noidung != value))
				{
					this.OnnoidungChanging(value);
					this.SendPropertyChanging();
					this._noidung = value;
					this.SendPropertyChanged("noidung");
					this.OnnoidungChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_chitiet", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string chitiet
		{
			get
			{
				return this._chitiet;
			}
			set
			{
				if ((this._chitiet != value))
				{
					this.OnchitietChanging(value);
					this.SendPropertyChanging();
					this._chitiet = value;
					this.SendPropertyChanged("chitiet");
					this.OnchitietChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.nguoidung")]
	public partial class nguoidung : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _tendangnhap;
		
		private string _matkhau;
		
		private string _sdt;
		
		private int _capdo;
		
		private int _kichhoat;
		
		private System.DateTime _ngaytao;
		
		private System.Nullable<System.DateTime> _lancuoi;
		
		private string _ghichu;
		
		private string _idtinh;
		
		private string _idhuyen;
		
		private string _idxa;
		
		private string _bhhuyen;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OntendangnhapChanging(string value);
    partial void OntendangnhapChanged();
    partial void OnmatkhauChanging(string value);
    partial void OnmatkhauChanged();
    partial void OnsdtChanging(string value);
    partial void OnsdtChanged();
    partial void OncapdoChanging(int value);
    partial void OncapdoChanged();
    partial void OnkichhoatChanging(int value);
    partial void OnkichhoatChanged();
    partial void OnngaytaoChanging(System.DateTime value);
    partial void OnngaytaoChanged();
    partial void OnlancuoiChanging(System.Nullable<System.DateTime> value);
    partial void OnlancuoiChanged();
    partial void OnghichuChanging(string value);
    partial void OnghichuChanged();
    partial void OnidtinhChanging(string value);
    partial void OnidtinhChanged();
    partial void OnidhuyenChanging(string value);
    partial void OnidhuyenChanged();
    partial void OnidxaChanging(string value);
    partial void OnidxaChanged();
    partial void OnbhhuyenChanging(string value);
    partial void OnbhhuyenChanged();
    #endregion
		
		public nguoidung()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tendangnhap", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string tendangnhap
		{
			get
			{
				return this._tendangnhap;
			}
			set
			{
				if ((this._tendangnhap != value))
				{
					this.OntendangnhapChanging(value);
					this.SendPropertyChanging();
					this._tendangnhap = value;
					this.SendPropertyChanged("tendangnhap");
					this.OntendangnhapChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_matkhau", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string matkhau
		{
			get
			{
				return this._matkhau;
			}
			set
			{
				if ((this._matkhau != value))
				{
					this.OnmatkhauChanging(value);
					this.SendPropertyChanging();
					this._matkhau = value;
					this.SendPropertyChanged("matkhau");
					this.OnmatkhauChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sdt", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string sdt
		{
			get
			{
				return this._sdt;
			}
			set
			{
				if ((this._sdt != value))
				{
					this.OnsdtChanging(value);
					this.SendPropertyChanging();
					this._sdt = value;
					this.SendPropertyChanged("sdt");
					this.OnsdtChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_capdo", DbType="Int NOT NULL")]
		public int capdo
		{
			get
			{
				return this._capdo;
			}
			set
			{
				if ((this._capdo != value))
				{
					this.OncapdoChanging(value);
					this.SendPropertyChanging();
					this._capdo = value;
					this.SendPropertyChanged("capdo");
					this.OncapdoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_kichhoat", DbType="Int NOT NULL")]
		public int kichhoat
		{
			get
			{
				return this._kichhoat;
			}
			set
			{
				if ((this._kichhoat != value))
				{
					this.OnkichhoatChanging(value);
					this.SendPropertyChanging();
					this._kichhoat = value;
					this.SendPropertyChanged("kichhoat");
					this.OnkichhoatChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ngaytao", DbType="DateTime NOT NULL")]
		public System.DateTime ngaytao
		{
			get
			{
				return this._ngaytao;
			}
			set
			{
				if ((this._ngaytao != value))
				{
					this.OnngaytaoChanging(value);
					this.SendPropertyChanging();
					this._ngaytao = value;
					this.SendPropertyChanged("ngaytao");
					this.OnngaytaoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_lancuoi", DbType="DateTime")]
		public System.Nullable<System.DateTime> lancuoi
		{
			get
			{
				return this._lancuoi;
			}
			set
			{
				if ((this._lancuoi != value))
				{
					this.OnlancuoiChanging(value);
					this.SendPropertyChanging();
					this._lancuoi = value;
					this.SendPropertyChanged("lancuoi");
					this.OnlancuoiChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ghichu", DbType="NVarChar(254) NOT NULL", CanBeNull=false)]
		public string ghichu
		{
			get
			{
				return this._ghichu;
			}
			set
			{
				if ((this._ghichu != value))
				{
					this.OnghichuChanging(value);
					this.SendPropertyChanging();
					this._ghichu = value;
					this.SendPropertyChanged("ghichu");
					this.OnghichuChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idtinh", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string idtinh
		{
			get
			{
				return this._idtinh;
			}
			set
			{
				if ((this._idtinh != value))
				{
					this.OnidtinhChanging(value);
					this.SendPropertyChanging();
					this._idtinh = value;
					this.SendPropertyChanged("idtinh");
					this.OnidtinhChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idhuyen", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string idhuyen
		{
			get
			{
				return this._idhuyen;
			}
			set
			{
				if ((this._idhuyen != value))
				{
					this.OnidhuyenChanging(value);
					this.SendPropertyChanging();
					this._idhuyen = value;
					this.SendPropertyChanged("idhuyen");
					this.OnidhuyenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idxa", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string idxa
		{
			get
			{
				return this._idxa;
			}
			set
			{
				if ((this._idxa != value))
				{
					this.OnidxaChanging(value);
					this.SendPropertyChanging();
					this._idxa = value;
					this.SendPropertyChanged("idxa");
					this.OnidxaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_bhhuyen", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string bhhuyen
		{
			get
			{
				return this._bhhuyen;
			}
			set
			{
				if ((this._bhhuyen != value))
				{
					this.OnbhhuyenChanging(value);
					this.SendPropertyChanging();
					this._bhhuyen = value;
					this.SendPropertyChanged("bhhuyen");
					this.OnbhhuyenChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dmxa")]
	public partial class dmxa : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _id;
		
		private string _ten;
		
		private string _idhuyen;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(string value);
    partial void OnidChanged();
    partial void OntenChanging(string value);
    partial void OntenChanged();
    partial void OnidhuyenChanging(string value);
    partial void OnidhuyenChanged();
    #endregion
		
		public dmxa()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", DbType="NVarChar(10) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ten", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string ten
		{
			get
			{
				return this._ten;
			}
			set
			{
				if ((this._ten != value))
				{
					this.OntenChanging(value);
					this.SendPropertyChanging();
					this._ten = value;
					this.SendPropertyChanged("ten");
					this.OntenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idhuyen", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string idhuyen
		{
			get
			{
				return this._idhuyen;
			}
			set
			{
				if ((this._idhuyen != value))
				{
					this.OnidhuyenChanging(value);
					this.SendPropertyChanging();
					this._idhuyen = value;
					this.SendPropertyChanged("idhuyen");
					this.OnidhuyenChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dmtinh")]
	public partial class dmtinh : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _id;
		
		private string _ten;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(string value);
    partial void OnidChanged();
    partial void OntenChanging(string value);
    partial void OntenChanged();
    #endregion
		
		public dmtinh()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", DbType="NVarChar(10) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ten", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string ten
		{
			get
			{
				return this._ten;
			}
			set
			{
				if ((this._ten != value))
				{
					this.OntenChanging(value);
					this.SendPropertyChanging();
					this._ten = value;
					this.SendPropertyChanged("ten");
					this.OntenChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dmhuyen")]
	public partial class dmhuyen : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _id;
		
		private string _ten;
		
		private string _idtinh;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(string value);
    partial void OnidChanged();
    partial void OntenChanging(string value);
    partial void OntenChanged();
    partial void OnidtinhChanging(string value);
    partial void OnidtinhChanged();
    #endregion
		
		public dmhuyen()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", DbType="NVarChar(10) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ten", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string ten
		{
			get
			{
				return this._ten;
			}
			set
			{
				if ((this._ten != value))
				{
					this.OntenChanging(value);
					this.SendPropertyChanging();
					this._ten = value;
					this.SendPropertyChanged("ten");
					this.OntenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_idtinh", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string idtinh
		{
			get
			{
				return this._idtinh;
			}
			set
			{
				if ((this._idtinh != value))
				{
					this.OnidtinhChanging(value);
					this.SendPropertyChanging();
					this._idtinh = value;
					this.SendPropertyChanged("idtinh");
					this.OnidtinhChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dmdantoc")]
	public partial class dmdantoc : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _id;
		
		private string _ten;
		
		private string _tenkhac;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(string value);
    partial void OnidChanged();
    partial void OntenChanging(string value);
    partial void OntenChanged();
    partial void OntenkhacChanging(string value);
    partial void OntenkhacChanged();
    #endregion
		
		public dmdantoc()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", DbType="NVarChar(5) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ten", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string ten
		{
			get
			{
				return this._ten;
			}
			set
			{
				if ((this._ten != value))
				{
					this.OntenChanging(value);
					this.SendPropertyChanging();
					this._ten = value;
					this.SendPropertyChanged("ten");
					this.OntenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tenkhac", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string tenkhac
		{
			get
			{
				return this._tenkhac;
			}
			set
			{
				if ((this._tenkhac != value))
				{
					this.OntenkhacChanging(value);
					this.SendPropertyChanging();
					this._tenkhac = value;
					this.SendPropertyChanged("tenkhac");
					this.OntenkhacChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dmbenhvien")]
	public partial class dmbenhvien : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _id;
		
		private string _matinh;
		
		private string _mabv;
		
		private string _ten;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(string value);
    partial void OnidChanged();
    partial void OnmatinhChanging(string value);
    partial void OnmatinhChanged();
    partial void OnmabvChanging(string value);
    partial void OnmabvChanged();
    partial void OntenChanging(string value);
    partial void OntenChanged();
    #endregion
		
		public dmbenhvien()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", DbType="NVarChar(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_matinh", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string matinh
		{
			get
			{
				return this._matinh;
			}
			set
			{
				if ((this._matinh != value))
				{
					this.OnmatinhChanging(value);
					this.SendPropertyChanging();
					this._matinh = value;
					this.SendPropertyChanged("matinh");
					this.OnmatinhChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_mabv", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string mabv
		{
			get
			{
				return this._mabv;
			}
			set
			{
				if ((this._mabv != value))
				{
					this.OnmabvChanging(value);
					this.SendPropertyChanging();
					this._mabv = value;
					this.SendPropertyChanged("mabv");
					this.OnmabvChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ten", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string ten
		{
			get
			{
				return this._ten;
			}
			set
			{
				if ((this._ten != value))
				{
					this.OntenChanging(value);
					this.SendPropertyChanging();
					this._ten = value;
					this.SendPropertyChanged("ten");
					this.OntenChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
