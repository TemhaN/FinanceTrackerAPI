USE [master]
GO
/****** Object:  Database [Finances]    Script Date: 10.03.2025 10:11:29 ******/
CREATE DATABASE [Finances]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Finances', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.TDG2022\MSSQL\DATA\Finances.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Finances_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.TDG2022\MSSQL\DATA\Finances_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Finances] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Finances].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Finances] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Finances] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Finances] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Finances] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Finances] SET ARITHABORT OFF 
GO
ALTER DATABASE [Finances] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [Finances] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Finances] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Finances] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Finances] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Finances] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Finances] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Finances] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Finances] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Finances] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Finances] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Finances] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Finances] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Finances] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Finances] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Finances] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Finances] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Finances] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Finances] SET  MULTI_USER 
GO
ALTER DATABASE [Finances] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Finances] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Finances] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Finances] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Finances] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Finances] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Finances] SET QUERY_STORE = ON
GO
ALTER DATABASE [Finances] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Finances]
GO
/****** Object:  Table [dbo].[admins]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[admins](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[course_categories]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[course_categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[courses]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[courses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[course_name] [nvarchar](200) NOT NULL,
	[description] [nvarchar](max) NULL,
	[video_url] [nvarchar](255) NULL,
	[category_id] [int] NOT NULL,
	[is_paid] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[currencies]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[currencies](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[currency_code] [nvarchar](10) NOT NULL,
	[currency_name] [nvarchar](50) NOT NULL,
	[exchange_rate] [decimal](18, 6) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[currency_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[expenses]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[expenses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[frequency] [nvarchar](50) NULL,
	[repeat_count] [int] NULL,
	[description] [nvarchar](255) NULL,
	[category_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[financial_operations]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[financial_operations](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[operation_name] [nvarchar](255) NULL,
	[description] [nvarchar](max) NULL,
	[category_id] [int] NULL,
	[operation_date] [datetime] NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[operation_type] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[incomes]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[incomes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[frequency] [nvarchar](50) NULL,
	[repeat_count] [int] NULL,
	[operation_category_id] [int] NOT NULL,
	[description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[instant_expenses]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[instant_expenses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[shop_name] [nvarchar](255) NULL,
	[purchase_date] [datetime] NULL,
	[amount] [decimal](15, 2) NOT NULL,
	[description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[operation_categories]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[operation_categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[paid_subscribers]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[paid_subscribers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[subscription_start_date] [datetime] NOT NULL,
	[subscription_end_date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_parameters]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_parameters](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[currency_id] [int] NOT NULL,
	[max_monthly_spending] [decimal](18, 2) NULL,
	[max_entertainment_spending] [decimal](18, 2) NULL,
	[max_savings_goal] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_sessions]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_sessions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[session_token] [nvarchar](255) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastAccessedAt] [datetime] NOT NULL,
	[ExpiresAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 10.03.2025 10:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[current_balance] [decimal](15, 2) NULL,
	[created_at] [datetime] NULL,
	[last_login] [datetime] NULL,
	[image_url] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[admins] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[courses] ADD  DEFAULT ((0)) FOR [is_paid]
GO
ALTER TABLE [dbo].[expenses] ADD  DEFAULT ((0)) FOR [repeat_count]
GO
ALTER TABLE [dbo].[financial_operations] ADD  DEFAULT (getdate()) FOR [operation_date]
GO
ALTER TABLE [dbo].[financial_operations] ADD  DEFAULT ('income') FOR [operation_type]
GO
ALTER TABLE [dbo].[incomes] ADD  DEFAULT ((0)) FOR [repeat_count]
GO
ALTER TABLE [dbo].[instant_expenses] ADD  DEFAULT (getdate()) FOR [purchase_date]
GO
ALTER TABLE [dbo].[user_sessions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[user_sessions] ADD  DEFAULT (getdate()) FOR [LastAccessedAt]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((0.00)) FOR [current_balance]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[courses]  WITH CHECK ADD FOREIGN KEY([category_id])
REFERENCES [dbo].[course_categories] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[expenses]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[expenses]  WITH CHECK ADD  CONSTRAINT [FK_expenses_operation_categories] FOREIGN KEY([category_id])
REFERENCES [dbo].[operation_categories] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[expenses] CHECK CONSTRAINT [FK_expenses_operation_categories]
GO
ALTER TABLE [dbo].[financial_operations]  WITH CHECK ADD FOREIGN KEY([category_id])
REFERENCES [dbo].[operation_categories] ([id])
GO
ALTER TABLE [dbo].[financial_operations]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[incomes]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[incomes]  WITH CHECK ADD  CONSTRAINT [FK_incomes_operation_categories] FOREIGN KEY([operation_category_id])
REFERENCES [dbo].[operation_categories] ([id])
GO
ALTER TABLE [dbo].[incomes] CHECK CONSTRAINT [FK_incomes_operation_categories]
GO
ALTER TABLE [dbo].[instant_expenses]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[paid_subscribers]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[user_parameters]  WITH CHECK ADD FOREIGN KEY([currency_id])
REFERENCES [dbo].[currencies] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[user_parameters]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[user_sessions]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[expenses]  WITH CHECK ADD CHECK  (([frequency]='yearly' OR [frequency]='monthly' OR [frequency]='weekly' OR [frequency]='daily'))
GO
ALTER TABLE [dbo].[financial_operations]  WITH CHECK ADD  CONSTRAINT [CK_financial_operations_operation_type] CHECK  (([operation_type]='expense' OR [operation_type]='income'))
GO
ALTER TABLE [dbo].[financial_operations] CHECK CONSTRAINT [CK_financial_operations_operation_type]
GO
ALTER TABLE [dbo].[incomes]  WITH CHECK ADD CHECK  (([frequency]='yearly' OR [frequency]='monthly' OR [frequency]='weekly' OR [frequency]='daily'))
GO
USE [master]
GO
ALTER DATABASE [Finances] SET  READ_WRITE 
GO
