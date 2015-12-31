using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using Funq.Abstract;

public interface __HandlerObject__<TKey>
{

}

public interface __Parent__
{
	
}

partial class __ListLikeClass__<T>
{
	protected override ISequentialBuilder<T, __ListLikeClass__<T>> EmptyBuilder {
		get;
	}


	protected override ISequentialBuilder<T, __ListLikeClass__<T>> BuilderFrom(__ListLikeClass__<T> collection) {
		throw new NotImplementedException();
	}

	public override IEnumerator<T> GetEnumerator() {
		throw new NotImplementedException();
	}
}



