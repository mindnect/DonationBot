using NAudio.Wave;
using System;
using System.IO;

namespace NVorbis.NAudioSupport
{
	internal class VorbisWaveReader : WaveStream, IDisposable, IWaveProvider, ISampleProvider
	{
		private VorbisReader _reader;

		private WaveFormat _waveFormat;

		[ThreadStatic]
		private static float[] _conversionBuffer;

		public override WaveFormat WaveFormat
		{
			get
			{
				return this._waveFormat;
			}
		}

		public override long Length
		{
			get
			{
				return (long)(this._reader.get_TotalTime().TotalSeconds * (double)this._waveFormat.get_SampleRate() * (double)this._waveFormat.get_Channels() * 4.0);
			}
		}

		public override long Position
		{
			get
			{
				return (long)(this._reader.get_DecodedTime().TotalMilliseconds * (double)this._reader.get_SampleRate() * (double)this._reader.get_Channels() * 4.0);
			}
			set
			{
				if (value < 0L || value > this.Length)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._reader.set_DecodedTime(TimeSpan.FromSeconds((double)value / (double)this._reader.get_SampleRate() / (double)this._reader.get_Channels() / 4.0));
			}
		}

		public bool IsParameterChange
		{
			get
			{
				return this._reader.get_IsParameterChange();
			}
		}

		public int StreamCount
		{
			get
			{
				return this._reader.get_StreamCount();
			}
		}

		public int? NextStreamIndex
		{
			get;
			set;
		}

		public int CurrentStream
		{
			get
			{
				return this._reader.get_StreamIndex();
			}
			set
			{
				if (!this._reader.SwitchStreams(value))
				{
					throw new InvalidDataException("The selected stream is not a valid Vorbis stream!");
				}
				if (this.NextStreamIndex.HasValue && value == this.NextStreamIndex.Value)
				{
					this.NextStreamIndex = null;
				}
			}
		}

		public int UpperBitrate
		{
			get
			{
				return this._reader.get_UpperBitrate();
			}
		}

		public int NominalBitrate
		{
			get
			{
				return this._reader.get_NominalBitrate();
			}
		}

		public int LowerBitrate
		{
			get
			{
				return this._reader.get_LowerBitrate();
			}
		}

		public string Vendor
		{
			get
			{
				return this._reader.get_Vendor();
			}
		}

		public string[] Comments
		{
			get
			{
				return this._reader.get_Comments();
			}
		}

		public long ContainerOverheadBits
		{
			get
			{
				return this._reader.get_ContainerOverheadBits();
			}
		}

		public IVorbisStreamStatus[] Stats
		{
			get
			{
				return this._reader.get_Stats();
			}
		}

		public VorbisWaveReader(string fileName)
		{
			this._reader = new VorbisReader(fileName);
			this._waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(this._reader.get_SampleRate(), this._reader.get_Channels());
		}

		public VorbisWaveReader(Stream sourceStream)
		{
			this._reader = new VorbisReader(sourceStream, false);
			this._waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(this._reader.get_SampleRate(), this._reader.get_Channels());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this._reader != null)
			{
				this._reader.Dispose();
				this._reader = null;
			}
			base.Dispose(disposing);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			count /= 4;
			count -= count % this._reader.get_Channels();
			float[] arg_2E_0;
			if ((arg_2E_0 = VorbisWaveReader._conversionBuffer) == null)
			{
				arg_2E_0 = (VorbisWaveReader._conversionBuffer = new float[count]);
			}
			float[] array = arg_2E_0;
			if (array.Length < count)
			{
				array = (VorbisWaveReader._conversionBuffer = new float[count]);
			}
			int num = this.Read(array, 0, count) * 4;
			Buffer.BlockCopy(array, 0, buffer, offset, num);
			return num;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			return this._reader.ReadSamples(buffer, offset, count);
		}

		public void ClearParameterChange()
		{
			this._reader.ClearParameterChange();
		}

		public bool GetNextStreamIndex()
		{
			if (!this.NextStreamIndex.HasValue)
			{
				int streamCount = this._reader.get_StreamCount();
				if (this._reader.FindNextStream())
				{
					this.NextStreamIndex = new int?(streamCount);
					return true;
				}
			}
			return false;
		}
	}
}
