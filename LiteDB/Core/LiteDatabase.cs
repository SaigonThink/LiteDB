﻿using LiteDB.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// The LiteDB database. Used for create a LiteDB instance and use all storage resoures. It's the database connection
    /// </summary>
    public partial class LiteDatabase : IDisposable
    {
        private LazyLoad<DbEngine> _engine;

        private BsonMapper _mapper;

        private Logger _log = new Logger();

        public Logger Log { get { return _log; } }

        /// <summary>
        /// Starts LiteDB database using a connectionString for filesystem database
        /// </summary>
        public LiteDatabase(string connectionString)
        {
            _engine = new LazyLoad<DbEngine>(
                () => new DbEngine(new FileDiskService(connectionString, _log), _log), 
                () => this.InitializeMapper(),
                () => this.InitializeDbVersion());
        }

        /// <summary>
        /// Initialize database using any read/write Stream (like MemoryStream)
        /// </summary>
        public LiteDatabase(Stream stream)
        {
            this.InitializeMapper();
            _engine = new LazyLoad<DbEngine>(
                () => new DbEngine(new StreamDiskService(stream), _log),
                () => this.InitializeMapper(),
                () => this.InitializeDbVersion());
        }

        /// <summary>
        /// Starts LiteDB database using full parameters
        /// </summary>
        public LiteDatabase(IDiskService diskService)
        {
            this.InitializeMapper();
            _engine = new LazyLoad<DbEngine>(
                () => new DbEngine(diskService, _log),
                () => this.InitializeMapper(),
                () => this.InitializeDbVersion());
        }

        public void Dispose()
        {
            if(_engine.IsValueCreated) _engine.Value.Dispose();
        }
    }
}
