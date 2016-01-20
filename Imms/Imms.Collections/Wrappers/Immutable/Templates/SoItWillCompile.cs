using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imms;
using Imms.Abstract;

internal interface __HandlerObject__<TKey>
{

}

internal interface __Parent__
{
	
}

internal partial class __ListLikeClass__<T>
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



