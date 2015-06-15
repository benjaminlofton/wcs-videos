package ban.client;

import com.amazonaws.util.IOUtils;
import com.fasterxml.jackson.databind.ObjectMapper;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.HttpClientBuilder;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.io.InputStream;
import java.rmi.server.ExportException;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by bnorrish on 6/14/15.
 */
@Component
public class WsdcRestClient {

  static private final String SEARCH_BY_FRAG_URL = "http://swingdancecouncil.herokuapp.com/pages/dancer_search_by_fragment.json?term=";

  public List<WsdcDancer> getDancersByFragment(String fragment) {

    String jsonResult = getByUrl(SEARCH_BY_FRAG_URL + fragment);

    ObjectMapper mapper = new ObjectMapper();
    List<WsdcDancer> dancers = new ArrayList<>();
    try {
      dancers = mapper.readValue(jsonResult, mapper.getTypeFactory().constructCollectionType(
          List.class, WsdcDancer.class));
    } catch (Exception ex) {
      throw new IllegalStateException("I pooped while converting JSON", ex);
    }

    return dancers;
  }


  private String getByUrl(String url) {

    // Spring RestTemplate would be nicer, but for some reason was returning NULL on all GET requests to swingdancecouncil.
    // Apache HttpClient is much lighter weight, but forces you to do some extra steps; Revisit this

    HttpClient client = HttpClientBuilder.create().build();

    HttpGet request = null;
    String result = null;
    try {
      request = new HttpGet(url);
      HttpResponse httpResponse = client.execute(request);
      InputStream entityContentStream = httpResponse.getEntity().getContent();
      result = IOUtils.toString(entityContentStream);
    } catch (IOException ex) {
      throw new IllegalStateException(ex);
    } finally {
      if (request != null) {
        request.releaseConnection();
      }
    }

    return result;
  }

}
