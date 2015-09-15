package ban.service.list;

import java.util.List;

/**
 * Created by bnorrish on 9/13/15.
 */
public interface ILister {

  List<String> getResults(Integer skip, Integer take);
  String getListType();
}
